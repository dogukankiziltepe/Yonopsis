using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Commands;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Queries;

namespace SiteYonetimi.API.Controllers;

/// <summary>Virman fişlerinin satırlarını tek tek (tazminat/icra/dönem detaylarıyla) yöneten ekran.</summary>
[Route("api/virman-fis-detaylari")]
[RequirePage("VirmanFisDetaylari")]
public class VirmanFisDetaylariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetVirmanSatirlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Handle(await Mediator.Send(new GetVirmanSatirByIdQuery(id, CurrentSiteId)));

    // "Virman Fişi" select'i için — HesaplarArasiVirman sayfa yetkisi olmayan kullanıcılar da seçebilsin
    [HttpGet("fisler")]
    public async Task<IActionResult> GetFisler()
        => Handle(await Mediator.Send(new GetVirmanSecimListesiQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveVirmanSatirDto dto)
    {
        var result = await Mediator.Send(new CreateVirmanSatirCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveVirmanSatirDto dto)
        => Handle(await Mediator.Send(new UpdateVirmanSatirCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteVirmanSatirCommand(id, CurrentSiteId)));
}
