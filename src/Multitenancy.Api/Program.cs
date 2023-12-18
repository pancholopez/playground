using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Middleware;
using Multitenancy.Api.Models;
using Multitenancy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITenantService, TenantService>();

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.MapGet("/", () => "Hello World!");

app.MapGet("/products", (IProductService productService) 
    => productService.GetAllProducts());

app.MapPost("/products", (ProductDto product, IProductService productService) 
    => productService.CreateProduct(product));

app.MapDelete("/products/{productId}", (int productId, IProductService productService)
    => productService.DeleteProduct(productId) ? Results.NoContent() : Results.NotFound());

app.Run();