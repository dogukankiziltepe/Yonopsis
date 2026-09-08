using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/gelir-tahsilat-makbuzlari")]
[RequirePage("TahsilatMakbuzlariGelir")]
public class GelirTahsilatMakbuzlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetGelirTahsilatMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGelirTahsilatMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateGelirTahsilatMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGelirTahsilatMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateGelirTahsilatMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteGelirTahsilatMakbuzuCommand(id, CurrentSiteId)));
}
