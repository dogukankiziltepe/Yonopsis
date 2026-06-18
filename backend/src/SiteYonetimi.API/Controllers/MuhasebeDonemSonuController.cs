using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Donemler.Commands;
using SiteYonetimi.Muhasebe.Donemler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe/donem-sonu")]
[RequirePage("MuhasebeDonemSonu")]
public class MuhasebeDonemSonuController : BaseController
{
    [HttpPost("onizleme")]
    public async Task<IActionResult> Onizleme([FromQuery] Guid donemId)
    {
        return Handle(await Mediator.Send(new GetKapanisOnizlemeQuery(CurrentSiteId, donemId)));
    }

    [HttpPost("kapanis")]
    public async Task<IActionResult> Kapanis([FromQuery] Guid donemId)
    {
        var result = await Mediator.Send(new DonemSonuKapanisCommand(CurrentSiteId, donemId));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(result.Data);
    }
}
