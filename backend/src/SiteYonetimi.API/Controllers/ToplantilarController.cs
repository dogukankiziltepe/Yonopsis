using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Toplantilar.Commands;
using SiteYonetimi.SiteManagement.Toplantilar.DTOs;
using SiteYonetimi.SiteManagement.Toplantilar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/toplantilar")]
[RequirePage("Toplantilar")]
public class ToplantilarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] ToplamtiDurum? durum = null)
        => Handle(await Mediator.Send(new GetToplantilarQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateToplamtiDto dto)
    {
        var result = await Mediator.Send(new CreateToplamtiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateToplamtiDto dto)
        => Handle(await Mediator.Send(new UpdateToplamtiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteToplamtiCommand(id, CurrentSiteId)));
}
