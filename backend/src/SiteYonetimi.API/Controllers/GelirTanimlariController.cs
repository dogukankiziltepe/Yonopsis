using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.GelirTanimlari.Commands;
using SiteYonetimi.SiteManagement.GelirTanimlari.DTOs;
using SiteYonetimi.SiteManagement.GelirTanimlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/gelir-tanimlari")]
[RequirePage("GelirTanimlari")]
public class GelirTanimlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetGelirTanimlariQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGelirTanimiDto dto)
    {
        var result = await Mediator.Send(new CreateGelirTanimiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGelirTanimiDto dto)
        => Handle(await Mediator.Send(new UpdateGelirTanimiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteGelirTanimiCommand(id, CurrentSiteId)));
}
