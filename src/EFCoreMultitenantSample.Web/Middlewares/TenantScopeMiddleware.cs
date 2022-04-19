using EFCoreMultitenantSample.Infrastructure.TenantSupport;

namespace EFCoreMultitenantSample.Web.Middlewares;

public class TenantScopeMiddleware
{
    private readonly RequestDelegate _next;

    public TenantScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, ITenantProvider tenantProvider)
    {
        if(httpContext.Request.Query.TryGetValue("tenant", out var tenantName))
        {
            using var scope = tenantProvider.BeginScope(tenantName);
            await _next(httpContext);

            return;
        }

        await _next(httpContext);
    }
}

/// <summary>
/// Extension method used to add the middleware to the HTTP request pipeline.
/// </summary>
public static class TenantScopeMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantScopeMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantScopeMiddleware>();
    }
}
