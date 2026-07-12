using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Personeller.Commands;
using SiteYonetimi.SiteManagement.Personeller.DTOs;
using SiteYonetimi.SiteManagement.Personeller.Queries;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Controllers;

[Route("api/personel")]
[RequirePage("Personel")]
public class PersonelController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
        => Handle(await Mediator.Send(new GetPersonellerQuery(CurrentSiteId, search, isActive, page, pageSize)));

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetail(Guid id)
        => Handle(await Mediator.Send(new GetPersonelFullDetailQuery(id, CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonelDto dto)
    {
        var result = await Mediator.Send(new CreatePersonelCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonelDto dto)
        => Handle(await Mediator.Send(new UpdatePersonelCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeletePersonelCommand(id, CurrentSiteId)));

    // ── Kimlik ────────────────────────────────────────────────────────
    [HttpPut("{id:guid}/kimlik")]
    public async Task<IActionResult> UpdateKimlik(Guid id, [FromBody] UpdatePersonelKimlikDto dto)
        => Handle(await Mediator.Send(new UpdatePersonelKimlikCommand(id, CurrentSiteId, dto)));

    // ── Muhasebe Entegrasyon ─────────────────────────────────────────
    [HttpPut("{id:guid}/muhasebe-entegrasyon")]
    public async Task<IActionResult> UpdateMuhasebeEntegrasyon(Guid id, [FromBody] UpdatePersonelMuhasebeEntegrasyonDto dto)
        => Handle(await Mediator.Send(new UpdatePersonelMuhasebeEntegrasyonCommand(id, CurrentSiteId, dto)));

    // ── Telefonlar ───────────────────────────────────────────────────
    public record TelefonRequest(string PhoneNumber, string? Label);

    [HttpPost("{id:guid}/telefonlar")]
    public async Task<IActionResult> AddTelefon(Guid id, [FromBody] TelefonRequest req)
    {
        var result = await Mediator.Send(new AddPersonelTelefonCommand(id, CurrentSiteId, req.PhoneNumber, req.Label));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("telefonlar/{telefonId:guid}")]
    public async Task<IActionResult> UpdateTelefon(Guid telefonId, [FromBody] TelefonRequest req)
        => Handle(await Mediator.Send(new UpdatePersonelTelefonCommand(telefonId, CurrentSiteId, req.PhoneNumber, req.Label)));

    [HttpDelete("telefonlar/{telefonId:guid}")]
    public async Task<IActionResult> DeleteTelefon(Guid telefonId)
        => Handle(await Mediator.Send(new DeletePersonelTelefonCommand(telefonId, CurrentSiteId)));

    // ── Acil Durum Kişileri ──────────────────────────────────────────
    public record AcilDurumKisiRequest(string AdSoyad, string? Yakinlik, string? Telefon);

    [HttpPost("{id:guid}/acil-durum-kisileri")]
    public async Task<IActionResult> AddAcilDurumKisi(Guid id, [FromBody] AcilDurumKisiRequest req)
    {
        var result = await Mediator.Send(new AddPersonelAcilDurumKisiCommand(id, CurrentSiteId, req.AdSoyad, req.Yakinlik, req.Telefon));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("acil-durum-kisileri/{kisiId:guid}")]
    public async Task<IActionResult> UpdateAcilDurumKisi(Guid kisiId, [FromBody] AcilDurumKisiRequest req)
        => Handle(await Mediator.Send(new UpdatePersonelAcilDurumKisiCommand(kisiId, CurrentSiteId, req.AdSoyad, req.Yakinlik, req.Telefon)));

    [HttpDelete("acil-durum-kisileri/{kisiId:guid}")]
    public async Task<IActionResult> DeleteAcilDurumKisi(Guid kisiId)
        => Handle(await Mediator.Send(new DeletePersonelAcilDurumKisiCommand(kisiId, CurrentSiteId)));

    // ── Eğitimler ────────────────────────────────────────────────────
    public record EgitimRequest(string EgitiminKonusu, string? Egitmen, string? EgitimYeri,
        DateOnly? BaslamaTarihi, DateOnly? BitisTarihi, decimal? ToplamSaat);

    [HttpPost("{id:guid}/egitimler")]
    public async Task<IActionResult> AddEgitim(Guid id, [FromBody] EgitimRequest req)
    {
        var result = await Mediator.Send(new AddPersonelEgitimCommand(
            id, CurrentSiteId, req.EgitiminKonusu, req.Egitmen, req.EgitimYeri, req.BaslamaTarihi, req.BitisTarihi, req.ToplamSaat));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("egitimler/{egitimId:guid}")]
    public async Task<IActionResult> UpdateEgitim(Guid egitimId, [FromBody] EgitimRequest req)
        => Handle(await Mediator.Send(new UpdatePersonelEgitimCommand(
            egitimId, CurrentSiteId, req.EgitiminKonusu, req.Egitmen, req.EgitimYeri, req.BaslamaTarihi, req.BitisTarihi, req.ToplamSaat)));

    [HttpDelete("egitimler/{egitimId:guid}")]
    public async Task<IActionResult> DeleteEgitim(Guid egitimId)
        => Handle(await Mediator.Send(new DeletePersonelEgitimCommand(egitimId, CurrentSiteId)));

    // ── İzinler ──────────────────────────────────────────────────────
    public record IzinRequest(DateOnly BaslangicTarihi, DateOnly BitisTarihi,
        PersonelIzinTuru IzinTuru, string? Aciklama);

    [HttpPost("{id:guid}/izinler")]
    public async Task<IActionResult> AddIzin(Guid id, [FromBody] IzinRequest req)
    {
        var result = await Mediator.Send(new AddPersonelIzinCommand(
            id, CurrentSiteId, req.BaslangicTarihi, req.BitisTarihi, req.IzinTuru, req.Aciklama));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("izinler/{izinId:guid}")]
    public async Task<IActionResult> UpdateIzin(Guid izinId, [FromBody] IzinRequest req)
        => Handle(await Mediator.Send(new UpdatePersonelIzinCommand(
            izinId, CurrentSiteId, req.BaslangicTarihi, req.BitisTarihi, req.IzinTuru, req.Aciklama)));

    [HttpDelete("izinler/{izinId:guid}")]
    public async Task<IActionResult> DeleteIzin(Guid izinId)
        => Handle(await Mediator.Send(new DeletePersonelIzinCommand(izinId, CurrentSiteId)));
}
