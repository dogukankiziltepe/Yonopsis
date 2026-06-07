using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.Auth.Commands;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Auth.Queries;

namespace SiteYonetimi.API.Controllers;

public class SiteSelectionController : BaseController
{
    [HttpGet("my-sites")]
    public async Task<IActionResult> GetMySites()
    {
        var result = await Mediator.Send(new GetUserSitesQuery(CurrentUserId));
        return Handle(result);
    }

    [HttpPost("select")]
    public async Task<IActionResult> SelectSite([FromBody] SelectSiteRequest request)
    {
        var result = await Mediator.Send(new SelectSiteCommand(CurrentUserId, request.SiteId, request.UserType));
        return Handle(result);
    }
}
