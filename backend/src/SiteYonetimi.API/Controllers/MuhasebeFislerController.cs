using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Muhasebe.Fisler.Commands;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Muhasebe.Fisler.Queries;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Controllers;

[Route("api/muhasebe")]
[RequirePage("MuhasebeFis")]
public class MuhasebeFislerController : BaseController
{
    [HttpGet("fisler")]
    public async Task<IActionResult> GetList(
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] FisTuru? tur = null,
        [FromQuery] FisDurumu? durum = null,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Handle(await Mediator.Send(
            new GetFisListQuery(CurrentSiteId, from, to, tur, durum, q, page, pageSize)));
    }

    [HttpGet("fisler/{id:guid}")]
    public async Task<IActionResult> GetDetay(Guid id)
    {
        return Handle(await Mediator.Send(new GetFisDetayQuery(CurrentSiteId, id)));
    }

    [HttpPost("fisler")]
    public async Task<IActionResult> Create([FromBody] CreateFisDto dto)
    {
        var result = await Mediator.Send(new CreateFisCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("fisler/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFisDto dto)
    {
        return Handle(await Mediator.Send(new UpdateFisCommand(CurrentSiteId, id, dto)));
    }

    [HttpPost("fisler/{id:guid}/onayla")]
    public async Task<IActionResult> Onayla(Guid id)
    {
        return Handle(await Mediator.Send(new OnaylaFisCommand(CurrentSiteId, id)));
    }

    [HttpPost("fisler/{id:guid}/iptal")]
    public async Task<IActionResult> Iptal(Guid id)
    {
        return Handle(await Mediator.Send(new IptalFisCommand(CurrentSiteId, id)));
    }

    [HttpDelete("fisler/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteFisCommand(CurrentSiteId, id)));
    }

    [HttpGet("fis-detaylari")]
    [RequirePage("MuhasebeFisDetay")]
    public async Task<IActionResult> GetFisDetaylari(
        [FromQuery] Guid? hesapId = null,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] FisDurumu? durum = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return Handle(await Mediator.Send(
            new GetFisDetaylarQuery(CurrentSiteId, hesapId, from, to, durum, page, pageSize)));
    }
}
