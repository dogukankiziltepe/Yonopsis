using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.DevirBakiyeleri.DTOs;

namespace SiteYonetimi.SiteManagement.DevirBakiyeleri.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateDevirBakiyeCommand(Guid SiteId, CreateDevirBakiyeDto Dto) : IRequest<Result<Guid>>;

public class CreateDevirBakiyeCommandHandler : IRequestHandler<CreateDevirBakiyeCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public CreateDevirBakiyeCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<Guid>> Handle(CreateDevirBakiyeCommand request, CancellationToken cancellationToken)
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

        var count = await _db.DevirBakiyeleri.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"DB-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new DevirBakiye
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            Tarih = request.Dto.Tarih,
            UnitId = request.Dto.UnitId,
            BorcluUserId = request.Dto.BorcluUserId,
            BorcluRol = request.Dto.BorcluRol,
            BorcluAdiSnapshot = borcluAdiSnapshot,
            Tutar = request.Dto.Tutar,
            Aciklama = request.Dto.Aciklama ?? "Devir"
        };

        _db.DevirBakiyeleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateDevirBakiyeCommand(Guid Id, Guid SiteId, UpdateDevirBakiyeDto Dto) : IRequest<Result>;

public class UpdateDevirBakiyeCommandHandler : IRequestHandler<UpdateDevirBakiyeCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public UpdateDevirBakiyeCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(UpdateDevirBakiyeCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.Tutar <= 0)
            return Result.Failure("Tutar sıfırdan büyük olmalıdır.");

        var entity = await _db.DevirBakiyeleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Devir bakiye kaydı bulunamadı.");

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
        entity.UnitId = request.Dto.UnitId;
        entity.BorcluUserId = request.Dto.BorcluUserId;
        entity.BorcluRol = request.Dto.BorcluRol;
        entity.BorcluAdiSnapshot = borcluAdiSnapshot;
        entity.Tutar = request.Dto.Tutar;
        entity.Aciklama = request.Dto.Aciklama ?? "Devir";
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteDevirBakiyeCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteDevirBakiyeCommandHandler : IRequestHandler<DeleteDevirBakiyeCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteDevirBakiyeCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteDevirBakiyeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DevirBakiyeleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Devir bakiye kaydı bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
