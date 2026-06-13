using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Hesaplar.Commands;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Muhasebe.Hesaplar.Queries;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe")]
[RequirePage("MuhasebeHesapPlani")]
public class MuhasebeHesaplarController : BaseController
{
    // ---- Hesap Planı ----

    [HttpGet("hesap-plani/tree")]
    public async Task<IActionResult> GetTree([FromQuery] bool sadeceAktif = false)
    {
        return Handle(await Mediator.Send(new GetHesapPlaniTreeQuery(CurrentSiteId, sadeceAktif)));
    }

    [HttpGet("hesaplar")]
    public async Task<IActionResult> GetHesaplar(
        [FromQuery] CariTuru? cariTuru = null,
        [FromQuery] bool? fisKesilebilir = null,
        [FromQuery] bool? aktif = null,
        [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(
            new GetHesapListQuery(CurrentSiteId, cariTuru, fisKesilebilir, aktif, search)));
    }

    [HttpGet("hesaplar/{id:guid}")]
    public async Task<IActionResult> GetHesapById(Guid id)
    {
        return Handle(await Mediator.Send(new GetHesapByIdQuery(CurrentSiteId, id)));
    }

    [HttpPost("hesaplar")]
    public async Task<IActionResult> CreateHesap([FromBody] CreateHesapDto dto)
    {
        var result = await Mediator.Send(new CreateHesapCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("hesaplar/{id:guid}")]
    public async Task<IActionResult> UpdateHesap(Guid id, [FromBody] UpdateHesapDto dto)
    {
        return Handle(await Mediator.Send(new UpdateHesapCommand(CurrentSiteId, id, dto)));
    }

    [HttpPatch("hesaplar/{id:guid}/aktif")]
    public async Task<IActionResult> ToggleAktif(Guid id, [FromQuery] bool aktif)
    {
        return Handle(await Mediator.Send(new ToggleHesapAktifCommand(CurrentSiteId, id, aktif)));
    }

    // ---- Cari Hesaplar ----

    [HttpGet("cari-hesaplar")]
    public async Task<IActionResult> GetCariHesaplar(
        [FromQuery] CariTuru? cariTuru = null,
        [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(new GetCariHesaplarQuery(CurrentSiteId, cariTuru, search)));
    }

    [HttpPost("cari-hesaplar")]
    public async Task<IActionResult> CreateCariHesap([FromBody] CreateCariHesapDto dto)
    {
        var result = await Mediator.Send(new CreateCariHesapCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }
}
