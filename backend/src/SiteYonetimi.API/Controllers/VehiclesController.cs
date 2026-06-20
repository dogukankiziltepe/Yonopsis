using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Vehicles.Commands;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;
using SiteYonetimi.SiteManagement.Vehicles.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/vehicles")]
[RequirePage("Araclar")]
public class VehiclesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(new GetVehiclesBySiteQuery(CurrentSiteId, page, pageSize, search)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetVehicleByIdQuery(id, CurrentSiteId)));
    }

    [HttpGet("by-unit/{unitId:guid}")]
    public async Task<IActionResult> GetByUnit(Guid unitId)
    {
        return Handle(await Mediator.Send(new GetVehiclesByUnitQuery(unitId, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleDto dto)
    {
        var result = await Mediator.Send(new CreateVehicleCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        return Handle(await Mediator.Send(new UpdateVehicleCommand(id, CurrentSiteId, dto)));
    }

    [HttpPatch("{id:guid}/assign-unit")]
    public async Task<IActionResult> AssignUnit(Guid id, [FromBody] AssignVehicleToUnitDto dto)
    {
        return Handle(await Mediator.Send(new AssignVehicleToUnitCommand(CurrentSiteId, id, dto)));
    }

    [HttpPatch("{id:guid}/owner")]
    public async Task<IActionResult> ChangeOwner(Guid id, [FromBody] ChangeVehicleOwnerDto dto)
    {
        return Handle(await Mediator.Send(new ChangeVehicleOwnerCommand(CurrentSiteId, id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteVehicleCommand(id, CurrentSiteId)));
    }
}
