using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Filters;

public class PermissionFilter : IAsyncActionFilter
{
    private readonly IPermissionService _permissionService;

    public PermissionFilter(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Sadece site token'ı olan request'lerde çalış
        var tokenType = context.HttpContext.User.FindFirst("tokenType")?.Value;
        if (tokenType != "site")
        {
            await next();
            return;
        }

        var actionAttr = context.ActionDescriptor.EndpointMetadata
            .OfType<RequirePageAttribute>()
            .FirstOrDefault();

        var controllerAttr = context.Controller.GetType()
            .GetCustomAttributes(typeof(RequirePageAttribute), inherit: true)
            .Cast<RequirePageAttribute>()
            .FirstOrDefault();

        var requirePage = actionAttr ?? controllerAttr;

        if (requirePage == null)
        {
            await next();
            return;
        }

        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            await WriteJsonAsync(context.HttpContext.Response, 401, "Kimlik doğrulaması gereklidir.");
            return;
        }

        var isSuperAdmin = context.HttpContext.User.FindFirst("isSuperAdmin")?.Value;
        if (string.Equals(isSuperAdmin, "True", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await WriteJsonAsync(context.HttpContext.Response, 401, "Kimlik doğrulaması gereklidir.");
            return;
        }

        var siteIdClaim = context.HttpContext.User.FindFirst("siteId")?.Value;
        if (!Guid.TryParse(siteIdClaim, out var siteId))
        {
            await WriteJsonAsync(context.HttpContext.Response, 400, "Geçersiz X-Site-Id header'ı.");
            return;
        }

        var method = context.HttpContext.Request.Method.ToUpperInvariant();
        var requiredLevel = method switch
        {
            "GET" => PermissionLevel.ReadOnly,
            "POST" => PermissionLevel.ReadAndCreate,
            _ => PermissionLevel.FullAccess
        };

        var userPermission = await _permissionService.GetUserPermissionAsync(userId, siteId, requirePage.PageName);

        if (userPermission < requiredLevel)
        {
            await WriteJsonAsync(context.HttpContext.Response, 403, "Bu işlem için yetkiniz bulunmamaktadır.");
            return;
        }

        await next();
    }

    private static async Task WriteJsonAsync(HttpResponse response, int statusCode, string message)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        await response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
