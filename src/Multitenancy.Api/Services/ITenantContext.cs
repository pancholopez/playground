namespace Multitenancy.Api.Services;

public interface ITenantContext
{
    public Guid TenantId { get; }
}