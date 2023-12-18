namespace Multitenancy.Api.Models;

public interface ITenantAware
{
    public Guid TenantId { get; set; }
}