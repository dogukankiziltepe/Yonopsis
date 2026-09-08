using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.IadeMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.IadeMakbuzlari.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateIadeMakbuzuCommand(Guid SiteId, CreateIadeMakbuzuDto Dto) : IRequest<Result<Guid>>;

public class CreateIadeMakbuzuCommandHandler : IRequestHandler<CreateIadeMakbuzuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public CreateIadeMakbuzuCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<Guid>> Handle(CreateIadeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result<Guid>.Failure("Tutar sıfırdan büyük olmalıdır.");

        string? borcluAdiSnapshot = null;
        if (request.Dto.BorcluUserId is Guid borcluUserId)
        {
            var user = await _masterDb.Users
                .Where(u => u.Id == borcluUserId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(cancellationToken);
            if (user is not null) borcluAdiSnapshot = $"{user.FirstName} {user.LastName}";
        }

        var count = await _db.IadeMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"IM-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new IadeMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            Tarih = request.Dto.Tarih,
            BorcluUserId = request.Dto.BorcluUserId,
            BorcluRol = request.Dto.BorcluRol,
            BorcluAdiSnapshot = borcluAdiSnapshot,
            KasaBankaId = request.Dto.KasaBankaId,
            Tutar = request.Dto.Tutar,
            Aciklama = request.Dto.Aciklama
        };

        _db.IadeMakbuzlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateIadeMakbuzuCommand(Guid Id, Guid SiteId, UpdateIadeMakbuzuDto Dto) : IRequest<Result>;

public class UpdateIadeMakbuzuCommandHandler : IRequestHandler<UpdateIadeMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public UpdateIadeMakbuzuCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(UpdateIadeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result.Failure("Tutar sıfırdan büyük olmalıdır.");

        var entity = await _db.IadeMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("İade makbuzu bulunamadı.");

        string? borcluAdiSnapshot = entity.BorcluAdiSnapshot;
        if (request.Dto.BorcluUserId != entity.BorcluUserId)
        {
            borcluAdiSnapshot = null;
            if (request.Dto.BorcluUserId is Guid borcluUserId)
            {
                var user = await _masterDb.Users
                    .Where(u => u.Id == borcluUserId)
                    .Select(u => new { u.FirstName, u.LastName })
                    .FirstOrDefaultAsync(cancellationToken);
                if (user is not null) borcluAdiSnapshot = $"{user.FirstName} {user.LastName}";
            }
        }

        entity.Tarih = request.Dto.Tarih;
        entity.BorcluUserId = request.Dto.BorcluUserId;
        entity.BorcluRol = request.Dto.BorcluRol;
        entity.BorcluAdiSnapshot = borcluAdiSnapshot;
        entity.KasaBankaId = request.Dto.KasaBankaId;
        entity.Tutar = request.Dto.Tutar;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteIadeMakbuzuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteIadeMakbuzuCommandHandler : IRequestHandler<DeleteIadeMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteIadeMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteIadeMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.IadeMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("İade makbuzu bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
