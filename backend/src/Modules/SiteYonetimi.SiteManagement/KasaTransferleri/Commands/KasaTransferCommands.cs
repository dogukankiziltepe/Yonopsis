using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.KasaTransferleri.DTOs;

namespace SiteYonetimi.SiteManagement.KasaTransferleri.Commands;

internal static class KasaTransferValidation
{
    public static async Task<string?> ValidateAsync(SharedTenantDbContext db, Guid siteId, decimal tutar, Guid cikis, Guid giris, CancellationToken ct)
    {
        if (tutar <= 0) return "Tutar sıfırdan büyük olmalıdır.";
        if (cikis == giris) return "Çıkış ve giriş kasası aynı olamaz.";
        var count = await db.KasaBanka.CountAsync(k => k.SiteId == siteId && (k.Id == cikis || k.Id == giris), ct);
        return count == 2 ? null : "Kasa/Banka bulunamadı.";
    }
}

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateKasaTransferCommand(Guid SiteId, CreateKasaTransferDto Dto) : IRequest<Result<Guid>>;

public class CreateKasaTransferCommandHandler : IRequestHandler<CreateKasaTransferCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateKasaTransferCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateKasaTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var error = await KasaTransferValidation.ValidateAsync(_db, request.SiteId, dto.Tutar, dto.CikisKasaBankaId, dto.GirisKasaBankaId, cancellationToken);
        if (error is not null) return Result<Guid>.Failure(error);

        var count = await _db.KasaTransferleri.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);

        var entity = new KasaTransfer
        {
            SiteId = request.SiteId,
            EvrakNo = $"KT-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}",
            BelgeNo = dto.BelgeNo,
            IslemTarihi = DateTime.UtcNow,
            Tarih = dto.Tarih,
            CikisKasaBankaId = dto.CikisKasaBankaId,
            GirisKasaBankaId = dto.GirisKasaBankaId,
            Tutar = dto.Tutar,
            Aciklama = dto.Aciklama
        };

        _db.KasaTransferleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateKasaTransferCommand(Guid Id, Guid SiteId, UpdateKasaTransferDto Dto) : IRequest<Result>;

public class UpdateKasaTransferCommandHandler : IRequestHandler<UpdateKasaTransferCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateKasaTransferCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateKasaTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var error = await KasaTransferValidation.ValidateAsync(_db, request.SiteId, dto.Tutar, dto.CikisKasaBankaId, dto.GirisKasaBankaId, cancellationToken);
        if (error is not null) return Result.Failure(error);

        var entity = await _db.KasaTransferleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Kasa transfer fişi bulunamadı.");

        entity.Tarih = dto.Tarih;
        entity.BelgeNo = dto.BelgeNo;
        entity.CikisKasaBankaId = dto.CikisKasaBankaId;
        entity.GirisKasaBankaId = dto.GirisKasaBankaId;
        entity.Tutar = dto.Tutar;
        entity.Aciklama = dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteKasaTransferCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteKasaTransferCommandHandler : IRequestHandler<DeleteKasaTransferCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteKasaTransferCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteKasaTransferCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.KasaTransferleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Kasa transfer fişi bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
