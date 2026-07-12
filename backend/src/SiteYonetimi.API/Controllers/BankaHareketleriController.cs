using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.BankaHareketleri.Commands;
using SiteYonetimi.SiteManagement.BankaHareketleri.DTOs;
using SiteYonetimi.SiteManagement.BankaHareketleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/banka-hareketleri")]
[RequirePage("BankaHareketleri")]
public class BankaHareketleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? kasaBankaId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Handle(await Mediator.Send(new GetBankaHareketleriQuery(CurrentSiteId, kasaBankaId, page, pageSize)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBankaHareketiDto dto)
    {
        var result = await Mediator.Send(new CreateBankaHareketiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBankaHareketiDto dto)
        => Handle(await Mediator.Send(new UpdateBankaHareketiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteBankaHareketiCommand(id, CurrentSiteId)));
}
