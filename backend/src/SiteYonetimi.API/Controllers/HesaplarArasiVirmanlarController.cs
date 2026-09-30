using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Commands;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/hesaplar-arasi-virman")]
[RequirePage("HesaplarArasiVirman")]
public class HesaplarArasiVirmanlarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetVirmanlarQuery(CurrentSiteId, page, pageSize, search)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Handle(await Mediator.Send(new GetVirmanByIdQuery(id, CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveVirmanDto dto)
    {
        var result = await Mediator.Send(new CreateVirmanCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveVirmanDto dto)
        => Handle(await Mediator.Send(new UpdateVirmanCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteVirmanCommand(id, CurrentSiteId)));
}
