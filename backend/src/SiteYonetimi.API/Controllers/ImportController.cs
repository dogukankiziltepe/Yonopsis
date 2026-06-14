using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Import.DTOs;
using SiteYonetimi.SiteManagement.Import.Services;

namespace SiteYonetimi.API.Controllers;

[Route("api/import")]
[RequirePage("Import")]
public class ImportController : BaseController
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string XlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IImportService _import;

    public ImportController(IImportService import)
    {
        _import = import;
    }

    [HttpGet("template/{type}")]
    public IActionResult Template(ImportType type)
    {
        var bytes = _import.GenerateTemplate(type);
        return File(bytes, XlsxMime, $"{type.ToString().ToLowerInvariant()}_template.xlsx");
    }

    [HttpPost("preview/{type}")]
    public async Task<IActionResult> Preview(ImportType type, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya gönderilmedi." });
        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "Dosya boyutu 5MB'ı geçemez." });

        await using var stream = file.OpenReadStream();
        var preview = await _import.PreviewAsync(CurrentSiteId, type, stream, ct);
        return Ok(preview);
    }

    [HttpPost("confirm/{type}")]
    public async Task<IActionResult> Confirm(ImportType type, [FromBody] ConfirmImportDto dto, CancellationToken ct)
    {
        var result = await _import.ConfirmAsync(CurrentSiteId, type, dto.Rows, ct);
        return Ok(result);
    }
}
