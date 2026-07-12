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
    public CreateTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.OdemeTutari <= 0)
            return Result<Guid>.Failure("Ödeme tutarı sıfırdan büyük olmalıdır.");

        var count = await _db.TahsilatMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);
        var evrakNo = $"TM-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new TahsilatMakbuzu
        {
            SiteId = request.SiteId,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            BorcluAdi = request.Dto.BorcluAdi,
            KasaBankaId = request.Dto.KasaBankaId,
            BorcMakbuzuId = request.Dto.BorcMakbuzuId,
            OdemeTutari = request.Dto.OdemeTutari,
            OdemeTipi = request.Dto.OdemeTipi,
            Aciklama = request.Dto.Aciklama
        };

        _db.TahsilatMakbuzlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateTahsilatMakbuzuCommand(Guid Id, Guid SiteId, UpdateTahsilatMakbuzuDto Dto) : IRequest<Result>;

public class UpdateTahsilatMakbuzuCommandHandler : IRequestHandler<UpdateTahsilatMakbuzuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTahsilatMakbuzuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTahsilatMakbuzuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.TahsilatMakbuzlari
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Tahsilat makbuzu bulunamadı.");

        entity.BorcluAdi = request.Dto.BorcluAdi;
        entity.KasaBankaId = request.Dto.KasaBankaId;
        entity.BorcMakbuzuId = request.Dto.BorcMakbuzuId;
        entity.OdemeTutari = request.Dto.OdemeTutari;
        entity.OdemeTipi = request.Dto.OdemeTipi;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

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

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
