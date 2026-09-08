using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BankaHareketleri.Commands;
using SiteYonetimi.SiteManagement.BankaHareketleri.DTOs;
using SiteYonetimi.SiteManagement.BankaHareketleri.ExcelImport.DTOs;
using SiteYonetimi.SiteManagement.BankaHareketleri.ExcelImport.Services;
using SiteYonetimi.SiteManagement.BankaHareketleri.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/banka-hareketleri")]
[RequirePage("BankaHareketleri")]
public class BankaHareketleriController : BaseController
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string XlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IBankaHareketiExcelImportService _excelImport;
    public BankaHareketleriController(IBankaHareketiExcelImportService excelImport) => _excelImport = excelImport;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? kasaBankaId = null,
        [FromQuery] BankaHareketiDurum? durum = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Handle(await Mediator.Send(new GetBankaHareketleriQuery(CurrentSiteId, kasaBankaId, durum, page, pageSize)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBankaHareketiDto dto)
    {
        var result = await Mediator.Send(new CreateBankaHareketiCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBankaHareketiDto dto)
        => Handle(await Mediator.Send(new UpdateBankaHareketiCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteBankaHareketiCommand(id, CurrentSiteId)));

    // ── Excel İçe Aktarma ────────────────────────────────────────────────────

    [HttpGet("excel-import/template")]
    public IActionResult ExcelImportTemplate()
    {
        var bytes = _excelImport.GenerateTemplate();
        return File(bytes, XlsxMime, "banka_hareketleri_sablonu.xlsx");
    }

    [HttpPost("excel-import/preview")]
    public async Task<IActionResult> ExcelImportPreview(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya gönderilmedi." });
        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "Dosya boyutu 5MB'ı geçemez." });

        await using var stream = file.OpenReadStream();
        var preview = await _excelImport.PreviewAsync(stream, ct);
        return Ok(preview);
    }

    [HttpPost("excel-import/confirm")]
    public async Task<IActionResult> ExcelImportConfirm([FromQuery] Guid kasaBankaId, [FromBody] BankaHareketiImportConfirmDto dto, CancellationToken ct)
        => Handle(await _excelImport.ConfirmAsync(CurrentSiteId, kasaBankaId, dto, ct));
}
