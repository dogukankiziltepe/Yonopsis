using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.OdemeMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.OdemeMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.OdemeMakbuzlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/odeme-makbuzlari")]
[RequirePage("OdemeMakbuzu")]
public class OdemeMakbuzlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetOdemeMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOdemeMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateOdemeMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOdemeMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateOdemeMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteOdemeMakbuzuCommand(id, CurrentSiteId)));
}
