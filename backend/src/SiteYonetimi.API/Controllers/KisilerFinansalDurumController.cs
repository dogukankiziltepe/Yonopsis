using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.KisilerFinansalDurum.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/kisiler-finansal-durum")]
[RequirePage("KisiFinansalDurum")]
public class KisilerFinansalDurumController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetKisilerFinansalDurumQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("{personUserId:guid}/detay")]
    public async Task<IActionResult> GetDetay(Guid personUserId)
        => Handle(await Mediator.Send(new GetKisiFinansalDetayQuery(CurrentSiteId, personUserId)));
}
