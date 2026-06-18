using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Parametreler.Commands;
using SiteYonetimi.Muhasebe.Parametreler.DTOs;
using SiteYonetimi.Muhasebe.Parametreler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe/parametreler")]
[RequirePage("MuhasebeParametre")]
public class MuhasebeParametreController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Handle(await Mediator.Send(new GetMuhasebeParametreQuery(CurrentSiteId)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMuhasebeParametreDto dto)
    {
        return Handle(await Mediator.Send(new UpdateMuhasebeParametreCommand(CurrentSiteId, dto)));
    }
}
