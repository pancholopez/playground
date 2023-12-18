using Multitenancy.Api.Services;

namespace Multitenancy.Api.Middleware;

// makes sure the tenant ID is available across http requests
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // if (context.User.Identity is { IsAuthenticated: true })
        // {
        //     var tenantId = context.User.Claims
        //         .FirstOrDefault(c => c.Type.Equals("tenantId", StringComparison.OrdinalIgnoreCase));
        //     if (tenantId != null)
        //     {
        //         context.Request.Headers.Append("X-Tenant-ID", tenantId.Value);
        //     }
        // }
        context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantFromHeader);
        if (Guid.TryParse(tenantFromHeader, out var tenantId))
        {
            await tenantService.SetTenantAsync(tenantId);
        }

        await _next(context);
    }

}