using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.BorcMakbuzlari.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateBorcMakbuzuCommand(Guid SiteId, CreateBorcMakbuzuDto Dto) : IRequest<Result<Guid>>;

public class CreateBorcMakbuzuCommandHandler : IRequestHandler<CreateBorcMakbuzuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateBorcMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateBorcMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result<Guid>.Failure("Tutar sıfırdan büyük olmalıdır.");

        var count = await _db.BorcMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"BM-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new BorcMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            Donem = request.Dto.Donem,
            SonOdemeTarihi = request.Dto.SonOdemeTarihi,
            UnitId = request.Dto.UnitId,
            BorcluAdi = request.Dto.BorcluAdi,
            GelirTanimiId = request.Dto.GelirTanimiId,
            Tutar = request.Dto.Tutar,
            Aciklama = request.Dto.Aciklama
        };

        _db.BorcMakbuzlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateBorcMakbuzuCommand(Guid Id, Guid SiteId, UpdateBorcMakbuzuDto Dto) : IRequest<Result>;

public class UpdateBorcMakbuzuCommandHandler : IRequestHandler<UpdateBorcMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateBorcMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateBorcMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BorcMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Borç makbuzu bulunamadı.");

        entity.Donem = request.Dto.Donem;
        entity.SonOdemeTarihi = request.Dto.SonOdemeTarihi;
        entity.UnitId = request.Dto.UnitId;
        entity.BorcluAdi = request.Dto.BorcluAdi;
        entity.GelirTanimiId = request.Dto.GelirTanimiId;
        entity.Tutar = request.Dto.Tutar;
        entity.GecikmeTutari = request.Dto.GecikmeTutari;
        entity.OdenenTutar = request.Dto.OdenenTutar;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteBorcMakbuzuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteBorcMakbuzuCommandHandler : IRequestHandler<DeleteBorcMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteBorcMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBorcMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BorcMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Borç makbuzu bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
