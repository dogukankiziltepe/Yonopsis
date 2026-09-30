using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Commands;

internal static class AvukatValidation
{
    public static string? Validate(SaveAvukatDto dto) =>
        string.IsNullOrWhiteSpace(dto.AdSoyad) ? "Avukat adı zorunludur." : null;
}

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateAvukatCommand(Guid SiteId, SaveAvukatDto Dto) : IRequest<Result<Guid>>;

public class CreateAvukatCommandHandler : IRequestHandler<CreateAvukatCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAvukatCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateAvukatCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var error = AvukatValidation.Validate(dto);
        if (error is not null) return Result<Guid>.Failure(error);

        var entity = new Avukat
        {
            SiteId = request.SiteId,
            AdSoyad = dto.AdSoyad.Trim(),
            BuroAdi = dto.BuroAdi,
            Telefon = dto.Telefon,
            Eposta = dto.Eposta,
            Adres = dto.Adres,
            IsActive = dto.IsActive
        };
        _db.Avukatlar.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateAvukatCommand(Guid Id, Guid SiteId, SaveAvukatDto Dto) : IRequest<Result>;

public class UpdateAvukatCommandHandler : IRequestHandler<UpdateAvukatCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAvukatCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAvukatCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var error = AvukatValidation.Validate(dto);
        if (error is not null) return Result.Failure(error);

        var entity = await _db.Avukatlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Avukat bulunamadı.");

        entity.AdSoyad = dto.AdSoyad.Trim();
        entity.BuroAdi = dto.BuroAdi;
        entity.Telefon = dto.Telefon;
        entity.Eposta = dto.Eposta;
        entity.Adres = dto.Adres;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteAvukatCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteAvukatCommandHandler : IRequestHandler<DeleteAvukatCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAvukatCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteAvukatCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Avukatlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Avukat bulunamadı.");
        if (await _db.IcraDosyalari.AnyAsync(d => d.AvukatId == entity.Id, cancellationToken))
            return Result.Failure("Bu avukata bağlı icra dosyaları var. Silmek yerine pasif yapabilirsiniz.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
