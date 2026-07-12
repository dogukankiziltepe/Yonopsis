using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Persons.Commands;
using SiteYonetimi.SiteManagement.Persons.DTOs;
using SiteYonetimi.SiteManagement.Persons.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/persons")]
[RequirePage("Kisiler")]
public class PersonsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(new GetSitePersonsQuery(CurrentSiteId, page, pageSize, search)));
    }

    [HttpGet("pending")]
    [RequirePage("Kisiler")]
    public async Task<IActionResult> GetPending()
    {
        return Handle(await Mediator.Send(new GetPendingPersonsQuery(CurrentSiteId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetPersonByIdQuery(id, CurrentSiteId)));
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetFullDetail(Guid id)
    {
        return Handle(await Mediator.Send(new GetPersonFullDetailQuery(id, CurrentSiteId)));
    }

    [HttpPut("{id:guid}/general")]
    public async Task<IActionResult> UpdateGeneral(Guid id, [FromBody] UpdatePersonGeneralInfoDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePersonGeneralInfoCommand(id, CurrentSiteId, dto)));
    }

    [HttpPut("{id:guid}/detail-info")]
    public async Task<IActionResult> UpdateDetailInfo(Guid id, [FromBody] UpdatePersonDetailInfoDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePersonDetailInfoCommand(id, CurrentSiteId, dto)));
    }

    [HttpPut("{id:guid}/identity")]
    public async Task<IActionResult> UpdateIdentity(Guid id, [FromBody] UpdatePersonIdentityInfoDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePersonIdentityInfoCommand(id, CurrentSiteId, dto)));
    }

    [HttpPost]
    public async Task<IActionResult> Invite([FromBody] InvitePersonDto dto)
    {
        var result = await Mediator.Send(new InvitePersonCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePersonCommand(id, CurrentSiteId, dto)));
    }

    [HttpPut("{id:guid}/approve")]
    [RequirePage("Kisiler")]
    public async Task<IActionResult> Approve(Guid id)
    {
        return Handle(await Mediator.Send(new UpdatePersonCommand(id, CurrentSiteId,
            new UpdatePersonDto(null, null, UserSiteStatus.Approved))));
    }

    [HttpPut("{id:guid}/reject")]
    [RequirePage("Kisiler")]
    public async Task<IActionResult> Reject(Guid id)
    {
        return Handle(await Mediator.Send(new UpdatePersonCommand(id, CurrentSiteId,
            new UpdatePersonDto(null, null, UserSiteStatus.Rejected))));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        return Handle(await Mediator.Send(new RemovePersonFromSiteCommand(id, CurrentSiteId)));
    }
}
