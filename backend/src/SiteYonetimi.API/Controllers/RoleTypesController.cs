using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Tenancy.RoleTypes.Commands;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;
using SiteYonetimi.Tenancy.RoleTypes.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/role-types")]
[RequirePage("RoleTipleri")]
public class RoleTypesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetRoleTypesBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(Guid id)
    {
        return Handle(await Mediator.Send(new GetRoleTypePermissionsQuery(id, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleTypeDto dto)
    {
        var result = await Mediator.Send(new CreateRoleTypeCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleTypeDto dto)
    {
        return Handle(await Mediator.Send(new UpdateRoleTypeCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteRoleTypeCommand(id, CurrentSiteId)));
    }

    [HttpPatch("{id:guid}/permissions")]
    public async Task<IActionResult> SetPermission(Guid id, [FromBody] SetRolePermissionDto dto)
    {
        return Handle(await Mediator.Send(new SetRolePermissionCommand(id, CurrentSiteId, dto)));
    }
}
