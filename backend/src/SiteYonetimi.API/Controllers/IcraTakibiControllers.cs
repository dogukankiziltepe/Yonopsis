using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.IcraTakibi.Commands;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;
using SiteYonetimi.SiteManagement.IcraTakibi.Queries;

namespace SiteYonetimi.API.Controllers;

// İcra Takibi grubunun her sayfası kendi Page yetkisiyle korunur; avukat select'i
// için her ekranın kendi "avukatlar" lookup'ı vardır (Avukatlar sayfası yetkisi gerekmez).

[Route("api/avukatlar")]
[RequirePage("Avukatlar")]
public class AvukatlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetAvukatlarQuery(CurrentSiteId, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveAvukatDto dto)
    {
        var result = await Mediator.Send(new CreateAvukatCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveAvukatDto dto)
        => Handle(await Mediator.Send(new UpdateAvukatCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteAvukatCommand(id, CurrentSiteId)));
}

[Route("api/takibe-gonder")]
[RequirePage("TakibeGonder")]
public class TakibeGonderController : BaseController
{
    [HttpGet("adaylar")]
    public async Task<IActionResult> GetAdaylar([FromQuery] Guid? buildingId, [FromQuery] Guid? unitId, [FromQuery] Guid? gelirTanimiId,
        [FromQuery] int gunSayisi = 0, [FromQuery] decimal enAzBakiye = 0)
        => Handle(await Mediator.Send(new GetTakipAdaylariQuery(CurrentSiteId,
            new TakipAdayFiltreDto(buildingId, unitId, gelirTanimiId, gunSayisi, enAzBakiye))));

    [HttpPost("baslat")]
    public async Task<IActionResult> Baslat([FromBody] TakipBaslatDto dto)
        => Handle(await Mediator.Send(new TakipBaslatCommand(CurrentSiteId, dto)));
}

[Route("api/icra-takipleri")]
[RequirePage("TakipListesi")]
public class IcraTakipleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null,
        [FromQuery] SiteYonetimi.Shared.Enums.TakipDurumu? durum = null)
        => Handle(await Mediator.Send(new GetTakiplerQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Handle(await Mediator.Send(new GetTakipByIdQuery(id, CurrentSiteId)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTakipDto dto)
        => Handle(await Mediator.Send(new UpdateTakipCommand(id, CurrentSiteId, dto)));

    [HttpPost("{id:guid}/icraya-ver")]
    public async Task<IActionResult> IcrayaVer(Guid id, [FromBody] IcrayaVerDto dto)
    {
        var result = await Mediator.Send(new IcrayaVerCommand(id, CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpGet("avukatlar")]
    public async Task<IActionResult> GetAvukatlar()
        => Handle(await Mediator.Send(new GetAvukatSecimListesiQuery(CurrentSiteId)));
}

[Route("api/icra-dosyalari")]
[RequirePage("IcraListesi")]
public class IcraDosyalariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetIcraDosyalariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Handle(await Mediator.Send(new GetIcraDosyasiByIdQuery(id, CurrentSiteId)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIcraDosyasiDto dto)
        => Handle(await Mediator.Send(new UpdateIcraDosyasiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteIcraDosyasiCommand(id, CurrentSiteId)));

    [HttpGet("avukatlar")]
    public async Task<IActionResult> GetAvukatlar()
        => Handle(await Mediator.Send(new GetAvukatSecimListesiQuery(CurrentSiteId)));
}

[Route("api/icra-raporu")]
[RequirePage("IcraRaporu")]
public class IcraRaporuController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? ilkTarih, [FromQuery] DateTime? sonTarih, [FromQuery] Guid? buildingId,
        [FromQuery] Guid? unitId, [FromQuery] Guid? borcluUserId, [FromQuery] Guid? avukatId, [FromQuery] string? dosyaNo)
        => Handle(await Mediator.Send(new GetIcraRaporuQuery(CurrentSiteId,
            new IcraRaporFiltreDto(ilkTarih, sonTarih, buildingId, unitId, borcluUserId, avukatId, dosyaNo))));

    [HttpGet("avukatlar")]
    public async Task<IActionResult> GetAvukatlar()
        => Handle(await Mediator.Send(new GetAvukatSecimListesiQuery(CurrentSiteId)));
}
