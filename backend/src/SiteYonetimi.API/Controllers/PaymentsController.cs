using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Payments.Commands;
using SiteYonetimi.SiteManagement.Payments.DTOs;
using SiteYonetimi.SiteManagement.Payments.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/payments")]
[RequirePage("Aidatlar")]
public class PaymentsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        return Handle(await Mediator.Send(new GetPaymentsBySiteQuery(CurrentSiteId, page, pageSize, search)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Handle(await Mediator.Send(new GetPaymentByIdQuery(id, CurrentSiteId)));
    }

    [HttpGet("by-unit/{unitId:guid}")]
    public async Task<IActionResult> GetByUnit(Guid unitId)
    {
        return Handle(await Mediator.Send(new GetPaymentsByUnitQuery(unitId, CurrentSiteId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
    {
        var result = await Mediator.Send(new CreatePaymentCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePaymentCommand(id, CurrentSiteId, dto)));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        return Handle(await Mediator.Send(new UpdatePaymentStatusCommand(id, CurrentSiteId, dto)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeletePaymentCommand(id, CurrentSiteId)));
    }
}
