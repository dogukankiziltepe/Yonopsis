using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.SiteManagement.Rezervasyonlar.Commands;
using SiteYonetimi.SiteManagement.Rezervasyonlar.DTOs;
using SiteYonetimi.SiteManagement.Rezervasyonlar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/rezervasyonlar")]
[RequirePage("Rezervasyonlar")]
public class RezervasyonlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? tesisId = null,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] RezervasyonDurum? durum = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
        => Handle(await Mediator.Send(new GetRezervasyonlarQuery(CurrentSiteId, tesisId, from, to, durum, page, pageSize)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRezervasyonDto dto)
    {
        var result = await Mediator.Send(new CreateRezervasyonCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRezervasyonDto dto)
        => Handle(await Mediator.Send(new UpdateRezervasyonCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteRezervasyonCommand(id, CurrentSiteId)));
}
