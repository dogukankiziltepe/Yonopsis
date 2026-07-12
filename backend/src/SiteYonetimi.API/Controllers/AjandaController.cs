using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.AjandaEtkinlikleri.Commands;
using SiteYonetimi.SiteManagement.AjandaEtkinlikleri.DTOs;
using SiteYonetimi.SiteManagement.AjandaEtkinlikleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/ajanda")]
[RequirePage("Ajanda")]
public class AjandaController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Handle(await Mediator.Send(new GetAjandaEtkinlikleriQuery(CurrentSiteId, page, pageSize, search, from, to)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAjandaEtkinlikDto dto)
    {
        var result = await Mediator.Send(new CreateAjandaEtkinlikCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAjandaEtkinlikDto dto)
        => Handle(await Mediator.Send(new UpdateAjandaEtkinlikCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteAjandaEtkinlikCommand(id, CurrentSiteId)));
}
