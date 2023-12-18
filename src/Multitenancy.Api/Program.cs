using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Middleware;
using Multitenancy.Api.Models;
using Multitenancy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// register db-context and db provider
builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// register services
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// setup cookie authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "multiTenantApp";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", async (LoginDto credentials, ILoginService loginService, HttpContext context) =>
{
    var userAccount = await loginService.LoginAsync(credentials.Email, credentials.Password);

    if (userAccount is null) return Results.Unauthorized();

    await context.SignInAsync(
        scheme: CookieAuthenticationDefaults.AuthenticationScheme,
        principal: new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userAccount.Id.ToString()),
            new Claim(ClaimTypes.Email, userAccount.Email),
            new Claim("Tenant", userAccount.TenantId.ToString())
        })),
        properties: new AuthenticationProperties());

    return Results.Ok(userAccount);
});

app.MapGet("/identity", (ClaimsPrincipal user) =>
{
    var email = user.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
    var tenantId = Guid.Parse(user.Claims.Single(x => x.Type == "Tenant").Value);

    return $"Hello {email} with tenant {tenantId}";
});

app.MapGet("/products", (IProductService productService)
    => productService.GetAllProducts());

app.MapPost("/products", (ProductDto product, IProductService productService)
    => productService.CreateProduct(product));

app.MapDelete("/products/{productId:int}", (int productId, IProductService productService)
    => productService.DeleteProduct(productId) ? Results.NoContent() : Results.NotFound());

app.Run();