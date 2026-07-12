using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.YapilacakIsler.Commands;
using SiteYonetimi.SiteManagement.YapilacakIsler.DTOs;
using SiteYonetimi.SiteManagement.YapilacakIsler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/yapilacak-isler")]
[RequirePage("YapilacakIsler")]
public class YapilacakIslerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, [FromQuery] YapilacakIsDurum? durum = null)
        => Handle(await Mediator.Send(new GetYapilacakIslerQuery(CurrentSiteId, page, pageSize, search, durum)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateYapilacakIsDto dto)
    {
        var result = await Mediator.Send(new CreateYapilacakIsCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateYapilacakIsDto dto)
        => Handle(await Mediator.Send(new UpdateYapilacakIsCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteYapilacakIsCommand(id, CurrentSiteId)));
}
