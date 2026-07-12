using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.AnaSayaclar.Commands;
using SiteYonetimi.SiteManagement.AnaSayaclar.DTOs;
using SiteYonetimi.SiteManagement.AnaSayaclar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/ana-sayaclar")]
[RequirePage("AnaSayac")]
public class AnaSayaclarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null, [FromQuery] SayacTipi? tip = null)
        => Handle(await Mediator.Send(new GetAnaSayaclarQuery(CurrentSiteId, page, pageSize, search, tip)));

    [HttpGet("all")]
    public async Task<IActionResult> GetAllList()
        => Handle(await Mediator.Send(new GetAllAnaSayaclarQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnaSayacDto dto)
    {
        var result = await Mediator.Send(new CreateAnaSayacCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnaSayacDto dto)
        => Handle(await Mediator.Send(new UpdateAnaSayacCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteAnaSayacCommand(id, CurrentSiteId)));
}
