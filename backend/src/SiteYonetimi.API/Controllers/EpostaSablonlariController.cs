using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.EpostaSablonlari.Commands;
using SiteYonetimi.SiteManagement.EpostaSablonlari.DTOs;
using SiteYonetimi.SiteManagement.EpostaSablonlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/eposta-sablonlari")]
[RequirePage("EpostaSablonlari")]
public class EpostaSablonlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetEpostaSablonlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpostaSablonuDto dto)
    {
        var result = await Mediator.Send(new CreateEpostaSablonuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEpostaSablonuDto dto)
        => Handle(await Mediator.Send(new UpdateEpostaSablonuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteEpostaSablonuCommand(id, CurrentSiteId)));
}
