using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Teklifler.Commands;
using SiteYonetimi.SiteManagement.Teklifler.DTOs;
using SiteYonetimi.SiteManagement.Teklifler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/teklifler")]
[RequirePage("Teklifler")]
public class TekliflerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] TeklifDurum? durum = null)
        => Handle(await Mediator.Send(new GetTekliflerQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeklifDto dto)
    {
        var result = await Mediator.Send(new CreateTeklifCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeklifDto dto)
        => Handle(await Mediator.Send(new UpdateTeklifCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteTeklifCommand(id, CurrentSiteId)));
}
