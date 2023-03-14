using System.IdentityModel.Tokens.Jwt;
using ImageGallery.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

const string authorityUrlKestrel = "https://localhost:5001";
const string authorityUrlIIS = "https://localhost:44310";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(configure => 
        configure.JsonSerializerOptions.PropertyNamingPolicy = null);

// clear default claims mapping. defaults were used for backwards compatibility by MS
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// add handler to add bearer token to the httpclient
builder.Services.AddAccessTokenManagement();

// create an HttpClient used for accessing the API
builder.Services.AddHttpClient("APIClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ImageGalleryAPIRoot"]);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddUserAccessTokenHandler(); // ensures the token is passed in every request

// HttpClient for revoking tokens
builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri(authorityUrlIIS);
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/Authentication/AccessDenied";
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = authorityUrlIIS;
        options.ClientId = "imagegalleryclient";
        options.ClientSecret = "secret";
        options.ResponseType = OpenIdConnectResponseType.Code;
        // options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
        // options.CallbackPath = new PathString("signin-oidc");
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        
        // remove extra claims to make token smaller
        options.ClaimActions.Remove("aud");
        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        
        // options.Scope.Add("imagegalleryapi.fullaccess");
        options.Scope.Add("imagegalleryapi.read");
        options.Scope.Add("imagegalleryapi.write");
        options.Scope.Add("country");
        options.Scope.Add("offline_access");
        options.Scope.Add("roles");
        
        options.ClaimActions.MapJsonKey("role", "role");
        options.ClaimActions.MapUniqueJsonKey("country", "country");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "given_name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Index}/{id?}");

app.Run();
