using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.GelirGruplari.Commands;
using SiteYonetimi.SiteManagement.GelirGruplari.DTOs;
using SiteYonetimi.SiteManagement.GelirGruplari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/gelir-gruplari")]
[RequirePage("GelirGruplari")]
public class GelirGruplariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetGelirGruplariQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGelirGrubuDto dto)
    {
        var result = await Mediator.Send(new CreateGelirGrubuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGelirGrubuDto dto)
        => Handle(await Mediator.Send(new UpdateGelirGrubuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteGelirGrubuCommand(id, CurrentSiteId)));
}
