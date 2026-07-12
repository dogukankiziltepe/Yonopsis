using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.KasaBanka.Commands;
using SiteYonetimi.SiteManagement.KasaBanka.DTOs;
using SiteYonetimi.SiteManagement.KasaBanka.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/kasa-banka")]
[RequirePage("KasaBanka")]
public class KasaBankaController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetKasaBankaQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKasaBankaDto dto)
    {
        var result = await Mediator.Send(new CreateKasaBankaCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKasaBankaDto dto)
        => Handle(await Mediator.Send(new UpdateKasaBankaCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteKasaBankaCommand(id, CurrentSiteId)));
}
