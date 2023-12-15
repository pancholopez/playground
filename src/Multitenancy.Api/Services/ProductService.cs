using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

/// <inheritdoc />
public class ProductService(ApplicationDbContext dbContext) : IProductService
{
    public IEnumerable<Product> GetAllProducts() => dbContext.Products.AsNoTracking().ToList();

    public Product CreateProduct(ProductDto product)
    {
        var newProduct = new Product
        {
            Name = product.Name,
            Description = product.Description
        };

        dbContext.Products.Add(newProduct);
        dbContext.SaveChanges();

        return newProduct;
    }

    public bool DeleteProduct(int productId)
    {
        var product = dbContext.Products.FirstOrDefault(x => x.Id == productId);

        if (product is null) return false;
        
        dbContext.Products.Remove(product);
        dbContext.SaveChanges();
        return true;
    }
}