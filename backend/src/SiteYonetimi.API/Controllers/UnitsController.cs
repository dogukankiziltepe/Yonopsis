using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Units.Commands;
using SiteYonetimi.SiteManagement.Units.DTOs;
using SiteYonetimi.SiteManagement.Units.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/units")]
[RequirePage("Daireler")]
public class UnitsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetBySite([FromQuery] Guid? buildingId)
    {
        return Handle(await Mediator.Send(new GetUnitsBySiteQuery(CurrentSiteId, buildingId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetUnitByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitDto dto)
    {
        var result = await Mediator.Send(new CreateUnitCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitDto dto)
    {
        return Handle(await Mediator.Send(new UpdateUnitCommand(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteUnitCommand(id)));
    }

    [HttpPatch("{id:guid}/assign-owner")]
    public async Task<IActionResult> AssignOwner(Guid id, [FromBody] AssignUserDto dto)
    {
        return Handle(await Mediator.Send(new AssignOwnerToUnitCommand(id, dto.UserId)));
    }

    [HttpPatch("{id:guid}/assign-tenant")]
    public async Task<IActionResult> AssignTenant(Guid id, [FromBody] AssignUserDto dto)
    {
        return Handle(await Mediator.Send(new AssignTenantToUnitCommand(id, dto.UserId)));
    }

    [HttpPatch("{id:guid}/unassign-owner")]
    public async Task<IActionResult> UnassignOwner(Guid id)
    {
        return Handle(await Mediator.Send(new UnassignOwnerFromUnitCommand(id)));
    }

    [HttpPatch("{id:guid}/unassign-tenant")]
    public async Task<IActionResult> UnassignTenant(Guid id)
    {
        return Handle(await Mediator.Send(new UnassignTenantFromUnitCommand(id)));
    }
}
