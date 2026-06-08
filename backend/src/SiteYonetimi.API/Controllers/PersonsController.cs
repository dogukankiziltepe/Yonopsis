using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetPersonByIdQuery(id, CurrentSiteId)));
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        return Handle(await Mediator.Send(new RemovePersonFromSiteCommand(id, CurrentSiteId)));
    }
}
