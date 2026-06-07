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
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetVehiclesBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetVehicleByIdQuery(id, CurrentSiteId)));
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteVehicleCommand(id, CurrentSiteId)));
    }
}
