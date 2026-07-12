using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.MobilBildirimSablonlari.Commands;
using SiteYonetimi.SiteManagement.MobilBildirimSablonlari.DTOs;
using SiteYonetimi.SiteManagement.MobilBildirimSablonlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/mobil-bildirim-sablonlari")]
[RequirePage("MobilBildirimSablonlari")]
public class MobilBildirimSablonlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetMobilBildirimSablonlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMobilBildirimSablonuDto dto)
    {
        var result = await Mediator.Send(new CreateMobilBildirimSablonuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMobilBildirimSablonuDto dto)
        => Handle(await Mediator.Send(new UpdateMobilBildirimSablonuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteMobilBildirimSablonuCommand(id, CurrentSiteId)));
}
