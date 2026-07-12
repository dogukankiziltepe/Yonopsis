using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.GiderGruplari.Commands;
using SiteYonetimi.SiteManagement.GiderGruplari.DTOs;
using SiteYonetimi.SiteManagement.GiderGruplari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/gider-gruplari")]
[RequirePage("GiderGruplari")]
public class GiderGruplariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetGiderGruplariQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGiderGrubuDto dto)
    {
        var result = await Mediator.Send(new CreateGiderGrubuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGiderGrubuDto dto)
        => Handle(await Mediator.Send(new UpdateGiderGrubuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteGiderGrubuCommand(id, CurrentSiteId)));
}
