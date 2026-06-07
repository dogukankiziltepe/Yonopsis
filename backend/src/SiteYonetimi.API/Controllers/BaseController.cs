using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SiteYonetimi.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected Guid CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

    protected string CurrentUserEmail =>
        User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    protected string CurrentUserType =>
        User.FindFirstValue("userType") ?? string.Empty;

    protected Guid CurrentSiteId =>
        Guid.TryParse(User.FindFirstValue("siteId"), out var id) ? id : Guid.Empty;

    protected Guid? CurrentRoleTypeId =>
        Guid.TryParse(User.FindFirstValue("roleTypeId"), out var id) ? id : null;

    protected bool HasSiteContext => CurrentSiteId != Guid.Empty;

    protected bool IsSuperAdmin =>
        string.Equals(User.FindFirstValue("isSuperAdmin"), "True", StringComparison.OrdinalIgnoreCase);

    protected IActionResult Handle<T>(SiteYonetimi.Shared.Common.Result<T> result)
    {
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(result.Data);
    }

    protected IActionResult Handle(SiteYonetimi.Shared.Common.Result result)
    {
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return NoContent();
    }
}
