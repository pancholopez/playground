using System.Diagnostics;
using System.Reflection;
using Auth.Playground.AspIdentityApp;
using Auth.Playground.AspIdentityApp.Data;
using Auth.Playground.AspIdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
// var tracePath = Path.Join(path, $"Log_AspIdentityApp_{DateTime.Now:yyyyMMdd-HHmm}.txt");
// Trace.Listeners.Add(new TextWriterTraceListener(System.IO.File.CreateText(tracePath)));
// Trace.AutoFlush = true;
var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(
        connectionString: "Data Source=AspIdentityAppDatabase.db", //todo: get from appsettings
        sqliteOptionsAction: opt=>opt.MigrationsAssembly(migrationAssembly));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 0;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.SignIn.RequireConfirmedEmail = true;

        options.User.RequireUniqueEmail = true;
        
        options.Tokens.EmailConfirmationTokenProvider = "email-conf"; //todo: maybe move this to constants?
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<EmailConfirmationTokenProvider<IdentityUser>>("email-conf")
    .AddPasswordValidator<DoesNotContainPasswordValidator<IdentityUser>>(); //TODO:validator for IBeenPwned

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});

builder.Services.Configure<EmailConfirmationTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(2);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddSingleton<IEmailService, InMemoryEmailService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// todo: change password hasher https://www.scottbrady91.com/aspnet-identity/improving-the-aspnet-core-identity-password-hasher

builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

//apply DB migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

//Seed auth data

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseSession();

app.Run();