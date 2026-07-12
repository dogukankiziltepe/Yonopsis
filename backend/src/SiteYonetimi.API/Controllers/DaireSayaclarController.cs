using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.DaireSayaclar.Commands;
using SiteYonetimi.SiteManagement.DaireSayaclar.DTOs;
using SiteYonetimi.SiteManagement.DaireSayaclar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/daire-sayaclar")]
[RequirePage("DaireSayaclari")]
public class DaireSayaclarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null, [FromQuery] Guid? anaSayacId = null,
        [FromQuery] SayacTipi? tip = null)
        => Handle(await Mediator.Send(new GetDaireSayaclarQuery(CurrentSiteId, page, pageSize, search, anaSayacId, tip)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDaireSayacDto dto)
    {
        var result = await Mediator.Send(new CreateDaireSayacCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDaireSayacDto dto)
        => Handle(await Mediator.Send(new UpdateDaireSayacCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteDaireSayacCommand(id, CurrentSiteId)));
}
