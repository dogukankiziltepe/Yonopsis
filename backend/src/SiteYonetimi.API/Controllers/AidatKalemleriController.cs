using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.AidatKalemleri.Commands;
using SiteYonetimi.SiteManagement.AidatKalemleri.DTOs;
using SiteYonetimi.SiteManagement.AidatKalemleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/aidat-kalemleri")]
[RequirePage("AidatKalemleri")]
public class AidatKalemleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetAidatKalemleriQuery(CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAidatKalemiDto dto)
    {
        var result = await Mediator.Send(new CreateAidatKalemiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAidatKalemiDto dto)
    {
        return Handle(await Mediator.Send(new UpdateAidatKalemiCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteAidatKalemiCommand(id, CurrentSiteId)));
    }
}
