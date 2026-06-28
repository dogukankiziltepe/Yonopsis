using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Tesisler.Commands;
using SiteYonetimi.SiteManagement.Tesisler.DTOs;
using SiteYonetimi.SiteManagement.Tesisler.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/tesisler")]
[RequirePage("Tesisler")]
public class TesislerController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Handle(await Mediator.Send(new GetTesislerQuery(CurrentSiteId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTesisDto dto)
    {
        var result = await Mediator.Send(new CreateTesisCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTesisDto dto)
        => Handle(await Mediator.Send(new UpdateTesisCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteTesisCommand(id, CurrentSiteId)));
}
