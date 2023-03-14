using Auth.Playground.CookieAuth.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(Constants.AuthenticationSchema)
    .AddCookie(Constants.AuthenticationSchema, options =>
    {
        options.Cookie.Name = Constants.AuthenticationSchema;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.AdminOnlyPolicyName, policy =>
    {
        policy.RequireClaim("Admin");
    });
    options.AddPolicy(Constants.MustBelongHrPolicyName, policy =>
    {
        policy.RequireClaim("Department", "HR");
    });
    options.AddPolicy(Constants.HrManagerOnlyPolicyName, policy =>
    {
        policy
            .RequireClaim("Department", "HR")
            .RequireClaim("HRManager")
            .Requirements.Add(new HrManagerProbationRequirement(1));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, HrManagerProbationRequirementHandler>();

builder.Services.AddRazorPages();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7153/");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

app.UseSession();

app.MapRazorPages();

app.Run();
