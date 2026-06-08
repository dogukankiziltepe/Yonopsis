using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Payments.Commands;
using SiteYonetimi.SiteManagement.Payments.DTOs;
using SiteYonetimi.SiteManagement.Payments.Queries;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

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

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreatePaymentsDto dto)
    {
        var result = await Mediator.Send(new BulkCreatePaymentsCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(new { count = result.Data, message = $"{result.Data} aidat kaydı oluşturuldu." });
    }

    [HttpPost("mark-overdue")]
    public async Task<IActionResult> MarkOverdue()
    {
        var result = await Mediator.Send(new MarkOverduePaymentsCommand(CurrentSiteId));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(new { count = result.Data, message = $"{result.Data} kayıt gecikmiş olarak işaretlendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Handle(await Mediator.Send(new DeletePaymentCommand(id, CurrentSiteId)));
    }

    [HttpGet("template")]
    public async Task<IActionResult> DownloadTemplate([FromQuery] List<string> items)
    {
        var result = await Mediator.Send(new GetPaymentTemplateQuery(CurrentSiteId, items));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return File(result.Data!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "aidat_sablonu.xlsx");
    }

    [HttpPost("import")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Import([FromForm] IFormFile file, [FromForm] DateTime dueDate, [FromForm] string? description)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "Dosya boş." });
        using var stream = file.OpenReadStream();
        var result = await Mediator.Send(new ImportPaymentsCommand(CurrentSiteId, stream, dueDate, description));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        var r = result.Data!;
        return Ok(new
        {
            created = r.Created,
            skipped = r.Skipped,
            errors = r.Errors,
            message = $"{r.Created} aidat oluşturuldu, {r.Skipped} satır atlandı."
        });
    }
}
