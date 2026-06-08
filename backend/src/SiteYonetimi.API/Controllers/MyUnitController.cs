using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.SiteManagement.Units.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/my-unit")]
public class MyUnitController : BaseController
{
    [HttpGet("as-owner")]
    public async Task<IActionResult> GetMyUnitsAsOwner()
    {
        return Handle(await Mediator.Send(new GetMyUnitsAsOwnerQuery(CurrentUserId, CurrentSiteId)));
    }

    [HttpGet("as-tenant")]
    public async Task<IActionResult> GetMyUnitAsTenant()
    {
        return Handle(await Mediator.Send(new GetMyUnitAsTenantQuery(CurrentUserId, CurrentSiteId)));
    }
}
