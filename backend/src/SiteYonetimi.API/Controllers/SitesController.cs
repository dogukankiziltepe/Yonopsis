using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.SiteManagement.Sites.Commands;
using SiteYonetimi.SiteManagement.Sites.DTOs;
using SiteYonetimi.SiteManagement.Sites.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/sites")]
public class SitesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new GetAllSitesQuery()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new GetSiteByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSiteDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        var result = await Mediator.Send(new CreateSiteCommand(dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSiteDto dto)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new UpdateSiteCommand(id, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!IsSuperAdmin) return Forbid();
        return Handle(await Mediator.Send(new DeleteSiteCommand(id)));
    }
}
