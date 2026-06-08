using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Infrastructure.Services;

namespace SiteYonetimi.API.Controllers;

[Route("api/files")]
public class FilesController : BaseController
{
    private readonly IFileStorageService _storage;
    private readonly SharedTenantDbContext _db;

    public FilesController(IFileStorageService storage, SharedTenantDbContext db)
    {
        _storage = storage;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Dosya seçilmedi." });
        try
        {
            var (storedFileName, contentType) = await _storage.SaveAsync(file, ct);
            var entity = new UploadedFile
            {
                SiteId = CurrentSiteId,
                UploadedByUserId = CurrentUserId,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                FileSize = file.Length
            };
            _db.UploadedFiles.Add(entity);
            await _db.SaveChangesAsync(ct);
            return Ok(new { id = entity.Id, fileName = file.FileName, size = file.Length });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var record = await _db.UploadedFiles
            .FirstOrDefaultAsync(f => f.Id == id && f.SiteId == CurrentSiteId, ct);
        if (record is null) return NotFound();
        try
        {
            var (stream, contentType, fileName) = _storage.GetFile(record.StoredFileName, record.OriginalFileName, record.ContentType);
            return File(stream, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var record = await _db.UploadedFiles
            .FirstOrDefaultAsync(f => f.Id == id && f.SiteId == CurrentSiteId, ct);
        if (record is null) return NotFound();
        await _storage.DeleteAsync(record.StoredFileName, ct);
        record.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
