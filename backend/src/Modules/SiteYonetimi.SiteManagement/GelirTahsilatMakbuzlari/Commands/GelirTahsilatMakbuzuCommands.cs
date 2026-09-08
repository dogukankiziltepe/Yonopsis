using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateGelirTahsilatMakbuzuCommand(Guid SiteId, CreateGelirTahsilatMakbuzuDto Dto) : IRequest<Result<Guid>>;

public class CreateGelirTahsilatMakbuzuCommandHandler : IRequestHandler<CreateGelirTahsilatMakbuzuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateGelirTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateGelirTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result<Guid>.Failure("Tutar sıfırdan büyük olmalıdır.");

        var cariExists = await _db.HesapPlani.AnyAsync(h => h.Id == request.Dto.CariHesapId && h.SiteId == request.SiteId, cancellationToken);
        if (!cariExists) return Result<Guid>.Failure("Cari hesap bulunamadı.");

        var count = await _db.GelirTahsilatMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"GT-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new GelirTahsilatMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            Tarih = request.Dto.Tarih,
            CariHesapId = request.Dto.CariHesapId,
            KasaBankaId = request.Dto.KasaBankaId,
            GelirTanimiId = request.Dto.GelirTanimiId,
            Tutar = request.Dto.Tutar,
            Aciklama = request.Dto.Aciklama,
            DagitimYapilacak = request.Dto.DagitimYapilacak
        };

        _db.GelirTahsilatMakbuzlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateGelirTahsilatMakbuzuCommand(Guid Id, Guid SiteId, UpdateGelirTahsilatMakbuzuDto Dto) : IRequest<Result>;

public class UpdateGelirTahsilatMakbuzuCommandHandler : IRequestHandler<UpdateGelirTahsilatMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateGelirTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateGelirTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result.Failure("Tutar sıfırdan büyük olmalıdır.");

        var entity = await _db.GelirTahsilatMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Tahsilat makbuzu bulunamadı.");

        entity.Tarih = request.Dto.Tarih;
        entity.CariHesapId = request.Dto.CariHesapId;
        entity.KasaBankaId = request.Dto.KasaBankaId;
        entity.GelirTanimiId = request.Dto.GelirTanimiId;
        entity.Tutar = request.Dto.Tutar;
        entity.Aciklama = request.Dto.Aciklama;
        entity.DagitimYapilacak = request.Dto.DagitimYapilacak;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteGelirTahsilatMakbuzuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteGelirTahsilatMakbuzuCommandHandler : IRequestHandler<DeleteGelirTahsilatMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteGelirTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteGelirTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GelirTahsilatMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Tahsilat makbuzu bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
