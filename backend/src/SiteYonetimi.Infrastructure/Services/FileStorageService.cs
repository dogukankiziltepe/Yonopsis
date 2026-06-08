using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SiteYonetimi.Infrastructure.Services;

public interface IFileStorageService
{
    Task<(string StoredFileName, string ContentType)> SaveAsync(IFormFile file, CancellationToken ct = default);
    Task DeleteAsync(string storedFileName, CancellationToken ct = default);
    (Stream Stream, string ContentType, string FileName) GetFile(string storedFileName, string originalFileName, string contentType);
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseDir;

    private static readonly HashSet<string> AllowedTypes =
    [
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain"
    ];

    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public LocalFileStorageService(IConfiguration configuration)
    {
        _baseDir = configuration["FileStorage:UploadPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_baseDir);
    }

    public async Task<(string StoredFileName, string ContentType)> SaveAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Dosya boş olamaz.");
        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("Dosya boyutu 10 MB'ı geçemez.");
        if (!AllowedTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("Bu dosya türüne izin verilmiyor.");

        var ext = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(_baseDir, storedFileName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, ct);

        return (storedFileName, file.ContentType);
    }

    public Task DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        var path = Path.Combine(_baseDir, storedFileName);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    public (Stream Stream, string ContentType, string FileName) GetFile(string storedFileName, string originalFileName, string contentType)
    {
        var path = Path.Combine(_baseDir, storedFileName);
        if (!File.Exists(path))
            throw new FileNotFoundException();
        return (File.OpenRead(path), contentType, originalFileName);
    }
}
