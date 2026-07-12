using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Faturalar.Commands;
using SiteYonetimi.SiteManagement.Faturalar.DTOs;
using SiteYonetimi.SiteManagement.Faturalar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/faturalar")]
public class FaturalarController : BaseController
{
    [HttpGet("gelir")]
    [RequirePage("GelirFaturalari")]
    public async Task<IActionResult> GetGelir([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetFaturalarQuery(CurrentSiteId, FaturaTipi.Gelir, page, pageSize, search)));

    [HttpGet("gider")]
    [RequirePage("GiderFaturalari")]
    public async Task<IActionResult> GetGider([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetFaturalarQuery(CurrentSiteId, FaturaTipi.Gider, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFaturaDto dto)
    {
        var result = await Mediator.Send(new CreateFaturaCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFaturaDto dto)
        => Handle(await Mediator.Send(new UpdateFaturaCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteFaturaCommand(id, CurrentSiteId)));
}
