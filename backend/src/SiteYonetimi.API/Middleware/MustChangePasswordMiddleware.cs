namespace SiteYonetimi.API.Middleware;

public class MustChangePasswordMiddleware
{
    private readonly RequestDelegate _next;

    public MustChangePasswordMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var mustChange = context.User.FindFirst("mustChangePassword")?.Value;
        if (!string.Equals(mustChange, "True", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        if (path.Equals("/api/auth/change-password", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new { message = "Şifrenizi değiştirmeniz gerekmektedir. Lütfen önce /api/auth/change-password endpoint'ini kullanın." });
    }
}
