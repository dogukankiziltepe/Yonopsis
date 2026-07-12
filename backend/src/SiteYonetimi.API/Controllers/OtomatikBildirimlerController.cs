using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.OtomatikBildirimler.Commands;
using SiteYonetimi.SiteManagement.OtomatikBildirimler.DTOs;
using SiteYonetimi.SiteManagement.OtomatikBildirimler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/otomatik-bildirimler")]
[RequirePage("OtomatikBildirimler")]
public class OtomatikBildirimlerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetOtomatikBildirimlerQuery(CurrentSiteId)));

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertOtomatikBildirimDto dto)
    {
        var result = await Mediator.Send(new UpsertOtomatikBildirimCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(new { id = result.Data });
    }
}
