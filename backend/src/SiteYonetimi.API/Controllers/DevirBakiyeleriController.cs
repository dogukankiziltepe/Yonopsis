using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.DevirBakiyeleri.Commands;
using SiteYonetimi.SiteManagement.DevirBakiyeleri.DTOs;
using SiteYonetimi.SiteManagement.DevirBakiyeleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/devir-bakiyeleri")]
[RequirePage("DevirBakiye")]
public class DevirBakiyeleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetDevirBakiyeleriQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDevirBakiyeDto dto)
    {
        var result = await Mediator.Send(new CreateDevirBakiyeCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDevirBakiyeDto dto)
        => Handle(await Mediator.Send(new UpdateDevirBakiyeCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteDevirBakiyeCommand(id, CurrentSiteId)));
}
