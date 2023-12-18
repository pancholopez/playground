using Microsoft.EntityFrameworkCore;
using Multitenancy.Api.Models;

namespace Multitenancy.Api.Services;

public class TenantService(ApplicationDbContext dbContext) : ITenantService
{
    private Guid _tenantId = Guid.Empty;

    public Guid GetTenantId() => _tenantId;

    public async Task SetTenantAsync(Guid tenantId)
    {
        var tenant = await dbContext.Tenants.FirstOrDefaultAsync(x => x.Id.Equals(tenantId));
        if (tenant is null) throw new InvalidOperationException($"Invalid Tenant {tenantId}");
        
        _tenantId = tenant.Id;
    }
}