using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Raporlar.Queries;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe")]
[RequirePage("MuhasebeRapor")]
public class MuhasebeRaporlarController : BaseController
{
    // ---- Defterler ----

    [HttpGet("defterler/yevmiye")]
    public async Task<IActionResult> Yevmiye([FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
    {
        return Handle(await Mediator.Send(new GetYevmiyeDefteriQuery(CurrentSiteId, from, to)));
    }

    [HttpGet("defterler/kebir")]
    public async Task<IActionResult> Kebir([FromQuery] Guid hesapId, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
    {
        return Handle(await Mediator.Send(new GetDefteriKebirQuery(CurrentSiteId, hesapId, from, to)));
    }

    [HttpGet("defterler/muavin")]
    public async Task<IActionResult> Muavin([FromQuery] Guid hesapId, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
    {
        return Handle(await Mediator.Send(new GetMuavinDefterQuery(CurrentSiteId, hesapId, from, to)));
    }

    // ---- Raporlar ----

    [HttpGet("raporlar/mizan")]
    public async Task<IActionResult> Mizan([FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
    {
        return Handle(await Mediator.Send(new GetMizanQuery(CurrentSiteId, from, to)));
    }

    [HttpGet("raporlar/cari-ekstre")]
    public async Task<IActionResult> CariEkstre([FromQuery] Guid hesapId, [FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null)
    {
        return Handle(await Mediator.Send(new GetCariEkstreQuery(CurrentSiteId, hesapId, from, to)));
    }

    [HttpGet("raporlar/borc-alacak")]
    public async Task<IActionResult> BorcAlacak([FromQuery] CariTuru? cariTuru = null)
    {
        return Handle(await Mediator.Send(new GetBorcAlacakDurumuQuery(CurrentSiteId, cariTuru)));
    }
}
