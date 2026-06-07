using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Buildings.Commands;
using SiteYonetimi.SiteManagement.Buildings.DTOs;
using SiteYonetimi.SiteManagement.Buildings.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/buildings")]
[RequirePage("Binalar")]
public class BuildingsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetBySite()
    {
        return Handle(await Mediator.Send(new GetBuildingsBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetBuildingByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBuildingDto dto)
    {
        var result = await Mediator.Send(new CreateBuildingCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBuildingDto dto)
    {
        return Handle(await Mediator.Send(new UpdateBuildingCommand(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteBuildingCommand(id)));
    }
}
