using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BirimFiyatlar.Commands;
using SiteYonetimi.SiteManagement.BirimFiyatlar.DTOs;
using SiteYonetimi.SiteManagement.BirimFiyatlar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/birim-fiyatlar")]
[RequirePage("BirimFiyat")]
public class BirimFiyatlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SayacTipi? tip = null)
        => Handle(await Mediator.Send(new GetBirimFiyatlarQuery(CurrentSiteId, tip)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBirimFiyatDto dto)
    {
        var result = await Mediator.Send(new CreateBirimFiyatCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBirimFiyatDto dto)
        => Handle(await Mediator.Send(new UpdateBirimFiyatCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteBirimFiyatCommand(id, CurrentSiteId)));
}
