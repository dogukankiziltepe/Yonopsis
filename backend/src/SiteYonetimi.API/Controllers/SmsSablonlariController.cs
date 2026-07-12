using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.SmsSablonlari.Commands;
using SiteYonetimi.SiteManagement.SmsSablonlari.DTOs;
using SiteYonetimi.SiteManagement.SmsSablonlari.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/sms-sablonlari")]
[RequirePage("SmsSablonlari")]
public class SmsSablonlariController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetSmsSablonlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSmsSablonuDto dto)
    {
        var result = await Mediator.Send(new CreateSmsSablonuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSmsSablonuDto dto)
        => Handle(await Mediator.Send(new UpdateSmsSablonuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteSmsSablonuCommand(id, CurrentSiteId)));
}
