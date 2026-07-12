using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Personeller.Commands;
using SiteYonetimi.SiteManagement.Personeller.DTOs;
using SiteYonetimi.SiteManagement.Personeller.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/personel")]
[RequirePage("Personel")]
public class PersonelController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
        => Handle(await Mediator.Send(new GetPersonellerQuery(CurrentSiteId, search, isActive, page, pageSize)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonelDto dto)
    {
        var result = await Mediator.Send(new CreatePersonelCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonelDto dto)
        => Handle(await Mediator.Send(new UpdatePersonelCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeletePersonelCommand(id, CurrentSiteId)));
}
