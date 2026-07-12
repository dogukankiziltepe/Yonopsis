using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.WebSiteAyarlari.Commands;
using SiteYonetimi.SiteManagement.WebSiteAyarlari.DTOs;
using SiteYonetimi.SiteManagement.WebSiteAyarlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/web-site-ayarlari")]
[RequirePage("AnaSayfaAyarlari")]
public class WebSiteAyarlariController : BaseController
{
    [HttpGet("ana-sayfa")]
    public async Task<IActionResult> GetAnaSayfa()
        => Handle(await Mediator.Send(new GetAnaSayfaAyarQuery(CurrentSiteId)));

    [HttpPut("ana-sayfa")]
    public async Task<IActionResult> UpdateAnaSayfa([FromBody] UpdateAnaSayfaAyarDto dto)
        => Handle(await Mediator.Send(new UpdateAnaSayfaAyarCommand(CurrentSiteId, dto)));

    [HttpGet("tema")]
    public async Task<IActionResult> GetTema()
        => Handle(await Mediator.Send(new GetSiteTemasQuery(CurrentSiteId)));

    [HttpPut("tema")]
    public async Task<IActionResult> UpdateTema([FromBody] UpdateSiteTemasDto dto)
        => Handle(await Mediator.Send(new UpdateSiteTemasCommand(CurrentSiteId, dto)));
}
