using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.IadeMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.IadeMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.IadeMakbuzlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/iade-makbuzlari")]
[RequirePage("IadeMakbuzu")]
public class IadeMakbuzlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetIadeMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIadeMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateIadeMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIadeMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateIadeMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteIadeMakbuzuCommand(id, CurrentSiteId)));
}
