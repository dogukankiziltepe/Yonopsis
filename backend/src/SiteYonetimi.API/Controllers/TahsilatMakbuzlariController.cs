using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.TahsilatMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.TahsilatMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.TahsilatMakbuzlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/tahsilat-makbuzlari")]
[RequirePage("TahsilatMakbuzu")]
public class TahsilatMakbuzlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetTahsilatMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTahsilatMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateTahsilatMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTahsilatMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateTahsilatMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteTahsilatMakbuzuCommand(id, CurrentSiteId)));
}
