using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Infrastructure.Data;

namespace SiteYonetimi.API.Middleware;

public class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, MasterDbContext db)
    {
        var ct = context.RequestAborted;
        // Sadece site token'ı olan request'lerde çalış
        var tokenType = context.User.FindFirst("tokenType")?.Value;
        if (tokenType != "site")
        {
            await _next(context);
            return;
        }

        var isSuperAdminClaim = context.User.FindFirst("isSuperAdmin")?.Value;
        if (string.Equals(isSuperAdminClaim, "True", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var siteIdClaim = context.User.FindFirst("siteId")?.Value;
        if (!Guid.TryParse(siteIdClaim, out var siteId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "Geçersiz X-Site-Id header'ı." });
            return;
        }

        var subscription = await db.SiteSubscriptions
            .Where(ss => ss.SiteId == siteId && ss.IsActive && ss.EndDate >= DateTime.UtcNow)
            .FirstOrDefaultAsync(ct);

        if (subscription == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Bu sitenin lisansı sona ermiştir." });
            return;
        }

        // Module erişim kontrolü
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var requirePage = actionDescriptor?.MethodInfo
            .GetCustomAttributes(typeof(RequirePageAttribute), inherit: false)
            .FirstOrDefault() as RequirePageAttribute
            ?? actionDescriptor?.ControllerTypeInfo
            .GetCustomAttributes(typeof(RequirePageAttribute), inherit: false)
            .FirstOrDefault() as RequirePageAttribute;

        if (requirePage != null)
        {
            var page = await db.Pages
                .FirstOrDefaultAsync(p => p.Name == requirePage.PageName && !p.IsDeleted, ct);

            if (page != null)
            {
                var allowed = await db.SubscriptionPlanModules
                    .AnyAsync(spm =>
                        spm.SubscriptionPlanId == subscription.SubscriptionPlanId &&
                        spm.ModuleId == page.ModuleId &&
                        !spm.IsDeleted, ct);

                if (!allowed)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "Bu özellik mevcut abonelik planınıza dahil değildir." });
                    return;
                }
            }
        }

        await _next(context);
    }
}
