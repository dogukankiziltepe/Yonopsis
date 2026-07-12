using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.FotografGalerisi.Commands;
using SiteYonetimi.SiteManagement.FotografGalerisi.DTOs;
using SiteYonetimi.SiteManagement.FotografGalerisi.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/fotograf-galerisi")]
[RequirePage("FotografGalerisi")]
public class FotografGalerisiController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetFotografGalerisiQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFotografGalerisiDto dto)
    {
        var result = await Mediator.Send(new CreateFotografGalerisiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFotografGalerisiDto dto)
        => Handle(await Mediator.Send(new UpdateFotografGalerisiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteFotografGalerisiCommand(id, CurrentSiteId)));
}
