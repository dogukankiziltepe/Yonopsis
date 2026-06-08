using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.API.Filters;
using SiteYonetimi.SiteManagement.Import.Commands;
using SiteYonetimi.SiteManagement.Import.DTOs;
using SiteYonetimi.SiteManagement.Import.Services;
using SiteYonetimi.SiteManagement.Import.Queries;

namespace SiteYonetimi.API.Controllers;

[Route("api/import")]
[RequirePage("TopluVeriGirisi")]
public class ImportController : BaseController
{
    private readonly ExcelTemplateService _templateService;

    public ImportController(ExcelTemplateService templateService)
    {
        _templateService = templateService;
    }

    /// <summary>Excel şablonu indir (type: buildings | units | users)</summary>
    [HttpGet("template/{type}")]
    public IActionResult DownloadTemplate(string type)
    {
        byte[] bytes;
        string fileName;

        switch (type.ToLower())
        {
            case "buildings":
                bytes = _templateService.CreateBuildingsTemplate();
                fileName = "buildings_template.xlsx";
                break;
            case "units":
                bytes = _templateService.CreateUnitsTemplate();
                fileName = "units_template.xlsx";
                break;
            case "users":
                bytes = _templateService.CreateUsersTemplate();
                fileName = "users_template.xlsx";
                break;
            default:
                return BadRequest(new { message = "Geçersiz tip. Geçerli değerler: buildings, units, users" });
        }

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>Excel dosyasını parse et ve önizleme döndür</summary>
    [HttpPost("preview/{type}")]
    [RequestSizeLimit(5_242_880)] // 5MB
    public async Task<IActionResult> Preview(string type, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya yüklenmedi." });

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Sadece .xlsx dosyaları kabul edilmektedir." });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var fileBytes = ms.ToArray();

        var result = await Mediator.Send(new PreviewImportQuery(CurrentSiteId, type, fileBytes));
        return Handle(result);
    }

    /// <summary>Seçilen bina satırlarını kaydet</summary>
    [HttpPost("confirm/buildings")]
    public async Task<IActionResult> ConfirmBuildings([FromBody] ConfirmBuildingsImportDto dto)
    {
        var result = await Mediator.Send(new ConfirmBuildingsImportCommand(CurrentSiteId, dto.Rows));
        return Handle(result);
    }

    /// <summary>Seçilen daire satırlarını kaydet</summary>
    [HttpPost("confirm/units")]
    public async Task<IActionResult> ConfirmUnits([FromBody] ConfirmUnitsImportDto dto)
    {
        var result = await Mediator.Send(new ConfirmUnitsImportCommand(CurrentSiteId, dto.Rows));
        return Handle(result);
    }

    /// <summary>Seçilen kullanıcı satırlarını kaydet</summary>
    [HttpPost("confirm/users")]
    public async Task<IActionResult> ConfirmUsers([FromBody] ConfirmUsersImportDto dto)
    {
        var result = await Mediator.Send(new ConfirmUsersImportCommand(CurrentSiteId, dto.Rows));
        return Handle(result);
    }
}
