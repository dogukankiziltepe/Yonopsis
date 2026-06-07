using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.Tenancy.Pages.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/pages")]
public class PagesController : BaseController
{
    [HttpGet("my-pages")]
    public async Task<IActionResult> GetMyPages()
    {
        var result = await Mediator.Send(new GetUserPagesQuery(CurrentUserId, CurrentSiteId, CurrentRoleTypeId, IsSuperAdmin));
        return Handle(result);
    }
}
