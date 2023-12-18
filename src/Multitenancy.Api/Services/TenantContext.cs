namespace Multitenancy.Api.Services;

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; }

    public TenantContext(IHttpContextAccessor contextAccessor)
    {
        var tenantClaim = contextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type.Equals("Tenant", StringComparison.OrdinalIgnoreCase));

        TenantId = Guid.TryParse(tenantClaim?.Value, out var tenantId) ? tenantId : Guid.Empty;
    }
}