namespace Multitenancy.Api.Models;

public class Product : ITenantAware
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid TenantId { get; set; }
}

public record ProductDto(string Name, string Description);