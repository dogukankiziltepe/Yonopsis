using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Anketler.Commands;
using SiteYonetimi.SiteManagement.Anketler.DTOs;
using SiteYonetimi.SiteManagement.Anketler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/anketler")]
[RequirePage("WebAnket")]
public class AnketlerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] AnketDurum? durum = null)
        => Handle(await Mediator.Send(new GetAnketlerQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnketDto dto)
    {
        var result = await Mediator.Send(new CreateAnketCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnketDto dto)
        => Handle(await Mediator.Send(new UpdateAnketCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteAnketCommand(id, CurrentSiteId)));
}
