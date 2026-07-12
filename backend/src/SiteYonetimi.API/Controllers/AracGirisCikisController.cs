using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.AracGirisCikis.Commands;
using SiteYonetimi.SiteManagement.AracGirisCikis.DTOs;
using SiteYonetimi.SiteManagement.AracGirisCikis.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/arac-giris-cikis")]
[RequirePage("AracGirisCikis")]
public class AracGirisCikisController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetAracGirisCikislarQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAracGirisCikisDto dto)
    {
        var result = await Mediator.Send(new CreateAracGirisCikisCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAracGirisCikisDto dto)
        => Handle(await Mediator.Send(new UpdateAracGirisCikisCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteAracGirisCikisCommand(id, CurrentSiteId)));
}
