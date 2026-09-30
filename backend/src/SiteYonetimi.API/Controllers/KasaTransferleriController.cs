using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.KasaTransferleri.Commands;
using SiteYonetimi.SiteManagement.KasaTransferleri.DTOs;
using SiteYonetimi.SiteManagement.KasaTransferleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/kasa-transferleri")]
[RequirePage("KasaTransfer")]
public class KasaTransferleriController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetKasaTransferleriQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Handle(await Mediator.Send(new GetKasaTransferByIdQuery(id, CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKasaTransferDto dto)
    {
        var result = await Mediator.Send(new CreateKasaTransferCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKasaTransferDto dto)
        => Handle(await Mediator.Send(new UpdateKasaTransferCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteKasaTransferCommand(id, CurrentSiteId)));
}
