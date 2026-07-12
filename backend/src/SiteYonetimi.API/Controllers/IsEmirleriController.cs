using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IsEmirleri.Commands;
using SiteYonetimi.SiteManagement.IsEmirleri.DTOs;
using SiteYonetimi.SiteManagement.IsEmirleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/is-emirleri")]
[RequirePage("IsTakipKayitlar")]
public class IsEmirleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] IsEmriDurum? durum = null,
        [FromQuery] Guid? departmanId = null)
        => Handle(await Mediator.Send(new GetIsEmirleriQuery(CurrentSiteId, page, pageSize, search, durum, departmanId)));

    [HttpGet("pano")]
    public async Task<IActionResult> GetPano()
        => Handle(await Mediator.Send(new GetIsEmirleriByDurumQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIsEmriDto dto)
    {
        var result = await Mediator.Send(new CreateIsEmriCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIsEmriDto dto)
        => Handle(await Mediator.Send(new UpdateIsEmriCommand(id, CurrentSiteId, dto)));

    [HttpPatch("{id:guid}/durum")]
    public async Task<IActionResult> UpdateDurum(Guid id, [FromBody] IsEmriDurum durum)
        => Handle(await Mediator.Send(new UpdateIsEmriDurumCommand(id, CurrentSiteId, durum)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteIsEmriCommand(id, CurrentSiteId)));
}
