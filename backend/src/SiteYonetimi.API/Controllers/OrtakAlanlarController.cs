using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.OrtakAlanlar.Commands;
using SiteYonetimi.SiteManagement.OrtakAlanlar.DTOs;
using SiteYonetimi.SiteManagement.OrtakAlanlar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/ortak-alanlar")]
[RequirePage("OrtakAlanlar")]
public class OrtakAlanlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetOrtakAlanlarQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("all")]
    public async Task<IActionResult> GetAllActive()
        => Handle(await Mediator.Send(new GetAllOrtakAlanlarQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrtakAlanDto dto)
    {
        var result = await Mediator.Send(new CreateOrtakAlanCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrtakAlanDto dto)
        => Handle(await Mediator.Send(new UpdateOrtakAlanCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteOrtakAlanCommand(id, CurrentSiteId)));
}
