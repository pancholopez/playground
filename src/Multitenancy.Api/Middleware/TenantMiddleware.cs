using Multitenancy.Api.Services;

namespace Multitenancy.Api.Middleware;

// makes sure the tenant ID is available across http requests for authenticated users
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            context.Request.Headers.Append("X-Tenant-ID", tenantContext.TenantId.ToString());
        }

        await _next(context);
    }
}