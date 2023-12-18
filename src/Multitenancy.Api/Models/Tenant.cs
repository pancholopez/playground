namespace Multitenancy.Api.Models;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();  // this should be auto-generated Id in database
    public string Name { get; set; }
}