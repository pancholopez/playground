using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

/// <summary>
/// Represents a service for managing products.
/// </summary>
public interface IProductService
{
    IEnumerable<Product> GetAllProducts();

    Product CreateProduct(ProductDto product);

    bool DeleteProduct(int productId);
}