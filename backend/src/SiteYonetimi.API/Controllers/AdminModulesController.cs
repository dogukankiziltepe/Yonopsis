using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.Tenancy.Plans.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/admin/modules")]
public class AdminModulesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new GetAllModulesQuery()));
    }
}
