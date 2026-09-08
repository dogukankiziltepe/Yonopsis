using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TahsilatMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.TahsilatMakbuzlari.Commands;

public record CreateTahsilatMakbuzuCommand(Guid SiteId, CreateTahsilatMakbuzuDto Dto) : IRequest<Result<Guid>>;

public class CreateTahsilatMakbuzuCommandHandler : IRequestHandler<CreateTahsilatMakbuzuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public CreateTahsilatMakbuzuCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<Guid>> Handle(CreateTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.OdemeTutari <= 0)
            return Result<Guid>.Failure("Ödeme tutarı sıfırdan büyük olmalıdır.");

        var count = await _db.TahsilatMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"TM-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        string? borcluAdiSnapshot = null;
        if (request.Dto.BorcluUserId is Guid borcluUserId)
        {
            var user = await _masterDb.Users
                .Where(u => u.Id == borcluUserId)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(cancellationToken);
            if (user is not null) borcluAdiSnapshot = $"{user.FirstName} {user.LastName}";
        }

        var entity = new TahsilatMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            BorcluUserId = request.Dto.BorcluUserId,
            BorcluAdiSnapshot = borcluAdiSnapshot,
            KasaBankaId = request.Dto.KasaBankaId,
            BorcMakbuzuId = request.Dto.BorcMakbuzuId,
            OdemeTutari = request.Dto.OdemeTutari,
            OdemeTipi = request.Dto.OdemeTipi,
            Aciklama = request.Dto.Aciklama
        };

        _db.TahsilatMakbuzlari.Add(entity);

        if (request.Dto.BorcMakbuzuId is Guid borcId)
        {
            var borc = await _db.BorcMakbuzlari.FirstOrDefaultAsync(
                x => x.Id == borcId && x.SiteId == request.SiteId, cancellationToken);
            if (borc is not null)
            {
                borc.OdenenTutar += request.Dto.OdemeTutari;
                borc.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateTahsilatMakbuzuCommand(Guid Id, Guid SiteId, UpdateTahsilatMakbuzuDto Dto) : IRequest<Result>;

public class UpdateTahsilatMakbuzuCommandHandler : IRequestHandler<UpdateTahsilatMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public UpdateTahsilatMakbuzuCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(UpdateTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.TahsilatMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Tahsilat makbuzu bulunamadı.");

        // Eski borca yansımış tutarı geri al
        if (entity.BorcMakbuzuId is Guid eskiBorcId)
        {
            var eskiBorc = await _db.BorcMakbuzlari.FirstOrDefaultAsync(
                x => x.Id == eskiBorcId && x.SiteId == request.SiteId, cancellationToken);
            if (eskiBorc is not null)
            {
                eskiBorc.OdenenTutar -= entity.OdemeTutari;
                eskiBorc.UpdatedAt = DateTime.UtcNow;
            }
        }

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

        entity.BorcluUserId = request.Dto.BorcluUserId;
        entity.BorcluAdiSnapshot = borcluAdiSnapshot;
        entity.KasaBankaId = request.Dto.KasaBankaId;
        entity.BorcMakbuzuId = request.Dto.BorcMakbuzuId;
        entity.OdemeTutari = request.Dto.OdemeTutari;
        entity.OdemeTipi = request.Dto.OdemeTipi;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        // Yeni borca yeni tutarı yansıt
        if (request.Dto.BorcMakbuzuId is Guid yeniBorcId)
        {
            var yeniBorc = await _db.BorcMakbuzlari.FirstOrDefaultAsync(
                x => x.Id == yeniBorcId && x.SiteId == request.SiteId, cancellationToken);
            if (yeniBorc is not null)
            {
                yeniBorc.OdenenTutar += request.Dto.OdemeTutari;
                yeniBorc.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteTahsilatMakbuzuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteTahsilatMakbuzuCommandHandler : IRequestHandler<DeleteTahsilatMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.TahsilatMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Tahsilat makbuzu bulunamadı.");

        if (entity.BorcMakbuzuId is Guid borcId)
        {
            var borc = await _db.BorcMakbuzlari.FirstOrDefaultAsync(
                x => x.Id == borcId && x.SiteId == request.SiteId, cancellationToken);
            if (borc is not null)
            {
                borc.OdenenTutar -= entity.OdemeTutari;
                borc.UpdatedAt = DateTime.UtcNow;
            }
        }

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
