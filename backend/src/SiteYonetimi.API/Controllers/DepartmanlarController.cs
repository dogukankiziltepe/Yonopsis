using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Departmanlar.Commands;
using SiteYonetimi.SiteManagement.Departmanlar.DTOs;
using SiteYonetimi.SiteManagement.Departmanlar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/departmanlar")]
[RequirePage("Departmanlar")]
public class DepartmanlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetDepartmanlarQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("all")]
    public async Task<IActionResult> GetAllActive()
        => Handle(await Mediator.Send(new GetAllDepartmanlarQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmanDto dto)
    {
        var result = await Mediator.Send(new CreateDepartmanCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmanDto dto)
        => Handle(await Mediator.Send(new UpdateDepartmanCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteDepartmanCommand(id, CurrentSiteId)));
}
