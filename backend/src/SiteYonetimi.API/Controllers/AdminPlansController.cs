using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.Tenancy.Plans.Commands;
using SiteYonetimi.Tenancy.Plans.DTOs;
using SiteYonetimi.Tenancy.Plans.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/admin/plans")]
public class AdminPlansController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new GetAllPlansQuery()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new GetPlanByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        var result = await Mediator.Send(new CreatePlanCommand(dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new UpdatePlanCommand(id, dto)));
    }

    [HttpPatch("{id:guid}/modules")]
    public async Task<IActionResult> SetModule(Guid id, [FromBody] SetPlanModuleDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new SetPlanModuleCommand(id, dto)));
    }
}
