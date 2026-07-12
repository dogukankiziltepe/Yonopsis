using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/borc-makbuzlari")]
[RequirePage("BorcMakbuzu")]
public class BorcMakbuzlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetBorcMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBorcMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateBorcMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBorcMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateBorcMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteBorcMakbuzuCommand(id, CurrentSiteId)));
}
