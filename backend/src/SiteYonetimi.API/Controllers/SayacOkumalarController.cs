using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.SayacOkumalar.Commands;
using SiteYonetimi.SiteManagement.SayacOkumalar.DTOs;
using SiteYonetimi.SiteManagement.SayacOkumalar.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/sayac-okumalar")]
[RequirePage("AnaSayac")]
public class SayacOkumalarController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 30,
        [FromQuery] Guid? anaSayacId = null, [FromQuery] Guid? daireSayacId = null)
        => Handle(await Mediator.Send(new GetSayacOkumalarQuery(CurrentSiteId, page, pageSize, anaSayacId, daireSayacId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSayacOkumaDto dto)
    {
        var result = await Mediator.Send(new CreateSayacOkumaCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSayacOkumaDto dto)
        => Handle(await Mediator.Send(new UpdateSayacOkumaCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteSayacOkumaCommand(id, CurrentSiteId)));
}
