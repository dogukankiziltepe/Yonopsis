using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SiteYonetimi.API.Filters;

public class AuditLogFilter : IAsyncActionFilter
{
    private readonly ILogger<AuditLogFilter> _logger;

    public AuditLogFilter(ILogger<AuditLogFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method.ToUpperInvariant();

        if (method == "GET")
        {
            await next();
            return;
        }

        var executedContext = await next();

        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var siteId = context.HttpContext.Request.Headers["X-Site-Id"].FirstOrDefault() ?? "-";
        var path = context.HttpContext.Request.Path;
        var statusCode = executedContext.HttpContext.Response.StatusCode;

        _logger.LogInformation(
            "AUDIT | UserId={UserId} | SiteId={SiteId} | Method={Method} | Path={Path} | StatusCode={StatusCode} | Time={Time}",
            userId, siteId, method, path, statusCode, DateTime.UtcNow);
    }
}
