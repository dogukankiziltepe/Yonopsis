using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.Commands;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.DTOs;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.Queries;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.TopluBorclandirma.DTOs;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.TopluBorclandirma.Services;

namespace SiteYonetimi.API.Controllers;

[Route("api/borc-makbuzlari")]
[RequirePage("BorcMakbuzu")]
public class BorcMakbuzlariController : BaseController
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string XlsxMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly ITopluBorclandirmaService _topluBorclandirma;
    public BorcMakbuzlariController(ITopluBorclandirmaService topluBorclandirma) => _topluBorclandirma = topluBorclandirma;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        => Handle(await Mediator.Send(new GetBorcMakbuzlariQuery(CurrentSiteId, page, pageSize, search)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBorcMakbuzuDto dto)
    {
        var result = await Mediator.Send(new CreateBorcMakbuzuCommand(CurrentSiteId, dto));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Created(string.Empty, new { id = result.Data });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBorcMakbuzuDto dto)
        => Handle(await Mediator.Send(new UpdateBorcMakbuzuCommand(id, CurrentSiteId, dto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => Handle(await Mediator.Send(new DeleteBorcMakbuzuCommand(id, CurrentSiteId)));

    // ── Toplu Borçlandırma ───────────────────────────────────────────────────

    [HttpGet("toplu-borclandirma/gelir-tanimlari-aktif")]
    [RequirePage("TopluBorclandirma")]
    public async Task<IActionResult> GetAktifGelirTanimlari(CancellationToken ct)
        => Ok(await _topluBorclandirma.GetAktifGelirTanimlariAsync(CurrentSiteId, ct));

    [HttpPost("toplu-borclandirma/template")]
    [RequirePage("TopluBorclandirma")]
    public async Task<IActionResult> TopluBorclandirmaTemplate([FromBody] TopluBorclandirmaTemplateRequestDto dto, CancellationToken ct)
    {
        var bytes = await _topluBorclandirma.GenerateTemplateAsync(CurrentSiteId, dto, ct);
        return File(bytes, XlsxMime, "toplu_borclandirma_sablonu.xlsx");
    }

    [HttpPost("toplu-borclandirma/preview")]
    [RequirePage("TopluBorclandirma")]
    public async Task<IActionResult> TopluBorclandirmaPreview(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya gönderilmedi." });
        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "Dosya boyutu 5MB'ı geçemez." });

        await using var stream = file.OpenReadStream();
        var preview = await _topluBorclandirma.PreviewAsync(CurrentSiteId, stream, ct);
        return Ok(preview);
    }

    [HttpPost("toplu-borclandirma/confirm")]
    [RequirePage("TopluBorclandirma")]
    public async Task<IActionResult> TopluBorclandirmaConfirm([FromBody] TopluBorclandirmaConfirmDto dto, CancellationToken ct)
        => Handle(await _topluBorclandirma.ConfirmAsync(CurrentSiteId, dto, ct));
}
