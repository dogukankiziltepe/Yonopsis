using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Olaylar.Commands;
using SiteYonetimi.SiteManagement.Olaylar.DTOs;
using SiteYonetimi.SiteManagement.Olaylar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/olaylar")]
[RequirePage("Olaylar")]
public class OlaylarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] OlayDurum? durum = null)
        => Handle(await Mediator.Send(new GetOlaylarQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOlayDto dto)
    {
        var result = await Mediator.Send(new CreateOlayCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOlayDto dto)
        => Handle(await Mediator.Send(new UpdateOlayCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteOlayCommand(id, CurrentSiteId)));
}
