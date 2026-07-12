using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.TelefonRehberi.Commands;
using SiteYonetimi.SiteManagement.TelefonRehberi.DTOs;
using SiteYonetimi.SiteManagement.TelefonRehberi.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/telefon-rehberi")]
[RequirePage("TelefonRehberi")]
public class TelefonRehberiController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetTelefonRehberiQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelefonRehberiDto dto)
    {
        var result = await Mediator.Send(new CreateTelefonRehberiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTelefonRehberiDto dto)
        => Handle(await Mediator.Send(new UpdateTelefonRehberiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteTelefonRehberiCommand(id, CurrentSiteId)));
}
