using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.SupportRequests.Commands;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;
using SiteYonetimi.SiteManagement.SupportRequests.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/support-requests")]
[RequirePage("DestekTalepleri")]
public class SupportRequestsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetSupportRequestsBySiteQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetSupportRequestByIdQuery(id, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupportRequestDto dto)
    {
        var result = await Mediator.Send(new CreateSupportRequestCommand(CurrentSiteId, CurrentUserId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupportRequestDto dto)
    {
        return Handle(await Mediator.Send(new UpdateSupportRequestCommand(id, CurrentSiteId, dto)));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSupportRequestStatusDto dto)
    {
        return Handle(await Mediator.Send(new UpdateSupportRequestStatusCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteSupportRequestCommand(id, CurrentSiteId)));
    }
}
