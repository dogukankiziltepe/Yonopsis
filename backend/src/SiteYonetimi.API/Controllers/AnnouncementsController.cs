using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Announcements.Commands;
using SiteYonetimi.SiteManagement.Announcements.DTOs;
using SiteYonetimi.SiteManagement.Announcements.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/announcements")]
[RequirePage("Duyurular")]
public class AnnouncementsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetAnnouncementsBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetAnnouncementByIdQuery(id, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
    {
        var result = await Mediator.Send(new CreateAnnouncementCommand(CurrentSiteId, CurrentUserId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnnouncementDto dto)
    {
        return Handle(await Mediator.Send(new UpdateAnnouncementCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteAnnouncementCommand(id, CurrentSiteId)));
    }
}
