using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OdemeMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.OdemeMakbuzlari.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateOdemeMakbuzuCommand(Guid SiteId, CreateOdemeMakbuzuDto Dto) : IRequest<Result<Guid>>;

public class CreateOdemeMakbuzuCommandHandler : IRequestHandler<CreateOdemeMakbuzuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateOdemeMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateOdemeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result<Guid>.Failure("Tutar sıfırdan büyük olmalıdır.");

        var cariExists = await _db.HesapPlani.AnyAsync(h => h.Id == request.Dto.CariHesapId && h.SiteId == request.SiteId, cancellationToken);
        if (!cariExists) return Result<Guid>.Failure("Cari hesap bulunamadı.");

        var count = await _db.OdemeMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"OM-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new OdemeMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            Tarih = request.Dto.Tarih,
            CariHesapId = request.Dto.CariHesapId,
            KasaBankaId = request.Dto.KasaBankaId,
            GiderTanimiId = request.Dto.GiderTanimiId,
            Tutar = request.Dto.Tutar,
            Aciklama = request.Dto.Aciklama,
            DagitimYapilacak = request.Dto.DagitimYapilacak
        };

        _db.OdemeMakbuzlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateOdemeMakbuzuCommand(Guid Id, Guid SiteId, UpdateOdemeMakbuzuDto Dto) : IRequest<Result>;

public class UpdateOdemeMakbuzuCommandHandler : IRequestHandler<UpdateOdemeMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateOdemeMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateOdemeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result.Failure("Tutar sıfırdan büyük olmalıdır.");

        var entity = await _db.OdemeMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Ödeme makbuzu bulunamadı.");

        entity.Tarih = request.Dto.Tarih;
        entity.CariHesapId = request.Dto.CariHesapId;
        entity.KasaBankaId = request.Dto.KasaBankaId;
        entity.GiderTanimiId = request.Dto.GiderTanimiId;
        entity.Tutar = request.Dto.Tutar;
        entity.Aciklama = request.Dto.Aciklama;
        entity.DagitimYapilacak = request.Dto.DagitimYapilacak;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteOdemeMakbuzuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteOdemeMakbuzuCommandHandler : IRequestHandler<DeleteOdemeMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteOdemeMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteOdemeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.OdemeMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Ödeme makbuzu bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
