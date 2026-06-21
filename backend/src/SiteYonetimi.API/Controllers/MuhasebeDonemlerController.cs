using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Donemler.Commands;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Muhasebe.Donemler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe/donemler")]
[RequirePage("MuhasebeFis")]
public class MuhasebeDonemlerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Handle(await Mediator.Send(new GetDonemlerQuery(CurrentSiteId)));
    }

    [HttpGet("aktif")]
    public async Task<IActionResult> GetAktif()
    {
        return Handle(await Mediator.Send(new GetAktifDonemQuery(CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDonemDto dto)
    {
        var result = await Mediator.Send(new CreateDonemCommand(CurrentSiteId, dto.Yil, dto.Ad));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }
}
