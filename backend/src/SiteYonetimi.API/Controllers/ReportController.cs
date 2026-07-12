using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Report.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/report")]
[RequirePage("Ozet")]
public class ReportController : BaseController
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
        => Handle(await Mediator.Send(new GetReportSummaryQuery(CurrentSiteId)));

    [HttpGet("kasalar")]
    public async Task<IActionResult> GetKasalar([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetKasalarQuery(CurrentSiteId, all, from, to)));

    [HttpGet("is-takibi")]
    public async Task<IActionResult> GetIsTakibi([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetIsTakibiQuery(CurrentSiteId, all, from, to)));

    [HttpGet("aidat-tahsilat")]
    public async Task<IActionResult> GetAidatTahsilat([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetAidatTahsilatQuery(CurrentSiteId, all, from, to)));

    [HttpGet("finansal-durum")]
    public async Task<IActionResult> GetFinansalDurum([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetFinansalDurumQuery(CurrentSiteId, all, from, to)));

    [HttpGet("gider-evraklari")]
    public async Task<IActionResult> GetGiderEvraklari()
        => Handle(await Mediator.Send(new GetGiderEvraklariQuery(CurrentSiteId)));

    [HttpGet("gelir-evraklari")]
    public async Task<IActionResult> GetGelirEvraklari()
        => Handle(await Mediator.Send(new GetGelirEvraklariQuery(CurrentSiteId)));

    [HttpGet("odenecek-faturalar")]
    public async Task<IActionResult> GetOdenecekFaturalar()
        => Handle(await Mediator.Send(new GetOdenecekFaturalarQuery(CurrentSiteId)));

    [HttpGet("gider-dagilimi")]
    public async Task<IActionResult> GetGiderDagilimi([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetGiderDagilimiQuery(CurrentSiteId, all, from, to)));

    [HttpGet("gelir-dagilimi")]
    public async Task<IActionResult> GetGelirDagilimi([FromQuery] bool all = false, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetGelirDagilimiQuery(CurrentSiteId, all, from, to)));

    [HttpGet("duyurular")]
    public async Task<IActionResult> GetDuyurular()
        => Handle(await Mediator.Send(new GetReportDuyurularQuery(CurrentSiteId)));

    [HttpGet("banka-hesaplari")]
    public async Task<IActionResult> GetBankaHesaplari()
        => Handle(await Mediator.Send(new GetReportBankaHesaplariQuery(CurrentSiteId)));
}
