namespace Multitenancy.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public record ProductDto(string Name, string Description);