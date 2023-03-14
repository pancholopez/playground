using System.IdentityModel.Tokens.Jwt;
using ImageGallery.API.Authorization;
using ImageGallery.API.DbContexts;
using ImageGallery.API.Services;
using ImageGallery.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

const string authorityUrlKestrel = "https://localhost:5001";
const string authorityUrlIIS = "https://localhost:44310";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<GalleryContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ImageGalleryDBConnectionString"]);
});

// register the repository
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, MustOwnImageHandler>();

// register AutoMapper-related services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     //.AddJwtBearer(options =>
     //{
     //    options.Authority = authorityUrlKestrel;
     //    options.Audience = "imagegalleryapi";
     //    options.TokenValidationParameters = new TokenValidationParameters
     //    {
     //        NameClaimType = "given_name",
     //        RoleClaimType = "role",
     //        ValidTypes = new[] { "at+jwt" }
     //    };
     //});
    .AddOAuth2Introspection(options =>
    {
        options.Authority = authorityUrlIIS;
        options.ClientId = "imagegalleryapi";
        options.ClientSecret = "apisecret";
        options.NameClaimType = "given_name";
        options.RoleClaimType = "role";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserCanAddImage", AuthorizationPolicies.CanAddImage());
    options.AddPolicy("ClientApplicationCanWrite", builder =>
    {
        builder.RequireClaim("scope", "imagegalleryapi.write");
    });
    options.AddPolicy("MustOwnImage", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.AddRequirements(new MustOwnImageRequirement());
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
