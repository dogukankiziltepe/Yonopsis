using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.GiderTanimlari.Commands;
using SiteYonetimi.SiteManagement.GiderTanimlari.DTOs;
using SiteYonetimi.SiteManagement.GiderTanimlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/gider-tanimlari")]
[RequirePage("GiderTanimlari")]
public class GiderTanimlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetGiderTanimlariQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGiderTanimiDto dto)
    {
        var result = await Mediator.Send(new CreateGiderTanimiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGiderTanimiDto dto)
        => Handle(await Mediator.Send(new UpdateGiderTanimiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteGiderTanimiCommand(id, CurrentSiteId)));
}
