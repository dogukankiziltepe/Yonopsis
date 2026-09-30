using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Commands;

internal static class IcraDosyasiValidation
{
    public static async Task<string?> ValidateAsync(SharedTenantDbContext db, Guid siteId, Guid? dosyaId, string dosyaNo, Guid? avukatId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dosyaNo)) return "Dosya numarası zorunludur.";
        var no = dosyaNo.Trim();
        if (await db.IcraDosyalari.AnyAsync(d => d.SiteId == siteId && d.DosyaNo == no && d.Id != dosyaId, ct))
            return "Bu dosya numarası ile kayıtlı başka bir icra dosyası var.";
        if (avukatId.HasValue && !await db.Avukatlar.AnyAsync(a => a.Id == avukatId && a.SiteId == siteId, ct))
            return "Avukat bulunamadı.";
        return null;
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateIcraDosyasiCommand(Guid Id, Guid SiteId, UpdateIcraDosyasiDto Dto) : IRequest<Result>;

public class UpdateIcraDosyasiCommandHandler : IRequestHandler<UpdateIcraDosyasiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateIcraDosyasiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateIcraDosyasiCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var dosya = await _db.IcraDosyalari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (dosya is null) return Result.Failure("İcra dosyası bulunamadı.");

        var error = await IcraDosyasiValidation.ValidateAsync(_db, request.SiteId, dosya.Id, dto.DosyaNo, dto.AvukatId, cancellationToken);
        if (error is not null) return Result.Failure(error);

        dosya.DosyaNo = dto.DosyaNo.Trim();
        dosya.IcraTarihi = dto.IcraTarihi.Date;
        dosya.Durum = dto.Durum;
        dosya.AvukatId = dto.AvukatId;
        dosya.Aciklama = dto.Aciklama;
        dosya.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
/// <summary>İcra dosyasını siler (soft); bağlı takip "İcraya Verilecek" durumuna geri döner.</summary>
public record DeleteIcraDosyasiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteIcraDosyasiCommandHandler : IRequestHandler<DeleteIcraDosyasiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteIcraDosyasiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteIcraDosyasiCommand request, CancellationToken cancellationToken)
    {
        var dosya = await _db.IcraDosyalari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (dosya is null) return Result.Failure("İcra dosyası bulunamadı.");

        dosya.IsDeleted = true;
        dosya.UpdatedAt = DateTime.UtcNow;

        if (dosya.TakipId.HasValue)
        {
            var takip = await _db.IcraTakipleri.FirstOrDefaultAsync(t => t.Id == dosya.TakipId, cancellationToken);
            if (takip is not null && takip.Durum == TakipDurumu.IcrayaVerildi)
            {
                takip.Durum = TakipDurumu.IcrayaVerilecek;
                takip.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
