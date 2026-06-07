using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.UnitTypes.Commands;
using SiteYonetimi.SiteManagement.UnitTypes.DTOs;
using SiteYonetimi.SiteManagement.UnitTypes.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/unit-types")]
[RequirePage("Daire Tipleri")]
public class UnitTypesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetBySite()
    {
        return Handle(await Mediator.Send(new GetUnitTypesBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetUnitTypeByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitTypeDto dto)
    {
        var result = await Mediator.Send(new CreateUnitTypeCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitTypeDto dto)
    {
        return Handle(await Mediator.Send(new UpdateUnitTypeCommand(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteUnitTypeCommand(id)));
    }
}
