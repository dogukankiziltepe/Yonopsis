using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.AccessCards.Commands;
using SiteYonetimi.SiteManagement.AccessCards.DTOs;
using SiteYonetimi.SiteManagement.AccessCards.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/access-cards")]
[RequirePage("GirisKartlari")]
public class AccessCardsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(new GetAccessCardsBySiteQuery(CurrentSiteId, page, pageSize, search)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetAccessCardByIdQuery(id, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccessCardDto dto)
    {
        var result = await Mediator.Send(new CreateAccessCardCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccessCardDto dto)
    {
        return Handle(await Mediator.Send(new UpdateAccessCardCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeleteAccessCardCommand(id, CurrentSiteId)));
    }
}
