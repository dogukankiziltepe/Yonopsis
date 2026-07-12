using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.KayipEsya.Commands;
using SiteYonetimi.SiteManagement.KayipEsya.DTOs;
using SiteYonetimi.SiteManagement.KayipEsya.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/kayip-esya")]
[RequirePage("KayipEsya")]
public class KayipEsyaController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] KayipEsyaDurum? durum = null)
        => Handle(await Mediator.Send(new GetKayipEsyalarQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKayipEsyaDto dto)
    {
        var result = await Mediator.Send(new CreateKayipEsyaCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKayipEsyaDto dto)
        => Handle(await Mediator.Send(new UpdateKayipEsyaCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteKayipEsyaCommand(id, CurrentSiteId)));
}
