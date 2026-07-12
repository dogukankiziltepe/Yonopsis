using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.TalepTipleri.Commands;
using SiteYonetimi.SiteManagement.TalepTipleri.DTOs;
using SiteYonetimi.SiteManagement.TalepTipleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/talep-tipleri")]
[RequirePage("TalepTipleri")]
public class TalepTipleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetTalepTipleriQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("all")]
    public async Task<IActionResult> GetAllActive()
        => Handle(await Mediator.Send(new GetAllTalepTipleriQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTalepTipiDto dto)
    {
        var result = await Mediator.Send(new CreateTalepTipiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTalepTipiDto dto)
        => Handle(await Mediator.Send(new UpdateTalepTipiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteTalepTipiCommand(id, CurrentSiteId)));
}
