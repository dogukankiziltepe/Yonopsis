using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.Commands;
using SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.DTOs;
using SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/ziyaretci-giris-cikis")]
[RequirePage("ZiyaretciGirisCikis")]
public class ZiyaretciGirisCikisController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetZiyaretciGirisCikislarQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateZiyaretciGirisCikisDto dto)
    {
        var result = await Mediator.Send(new CreateZiyaretciGirisCikisCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateZiyaretciGirisCikisDto dto)
        => Handle(await Mediator.Send(new UpdateZiyaretciGirisCikisCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteZiyaretciGirisCikisCommand(id, CurrentSiteId)));
}
