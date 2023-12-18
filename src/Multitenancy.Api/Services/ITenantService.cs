namespace Multitenancy.Api.Services;

public interface ITenantService
{
    public Guid GetTenantId();
    public Task SetTenantAsync(Guid tenantId);
}