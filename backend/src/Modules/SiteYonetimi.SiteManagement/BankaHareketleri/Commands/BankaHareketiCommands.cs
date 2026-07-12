using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BankaHareketleri.DTOs;

namespace SiteYonetimi.SiteManagement.BankaHareketleri.Commands;

public record CreateBankaHareketiCommand(Guid SiteId, CreateBankaHareketiDto Dto) : IRequest<Result<Guid>>;

public class CreateBankaHareketiCommandHandler : IRequestHandler<CreateBankaHareketiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateBankaHareketiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateBankaHareketiCommand request, CancellationToken cancellationToken)
    {
        var kb = await _db.KasaBanka.FindAsync(new object[] { request.Dto.KasaBankaId }, cancellationToken);
        if (kb == null || kb.SiteId != request.SiteId)
            return Result<Guid>.Failure("Kasa/Banka hesabı bulunamadı.");

        var entity = new BankaHareketi
        {
            SiteId = request.SiteId,
            KasaBankaId = request.Dto.KasaBankaId,
            Tarih = request.Dto.Tarih,
            Aciklama = request.Dto.Aciklama,
            ReferansNo = request.Dto.ReferansNo,
            Tutar = request.Dto.Tutar,
            Durum = BankaHareketiDurum.Bekleyen
        };

        _db.BankaHareketleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateBankaHareketiCommand(Guid Id, Guid SiteId, UpdateBankaHareketiDto Dto) : IRequest<Result>;

public class UpdateBankaHareketiCommandHandler : IRequestHandler<UpdateBankaHareketiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateBankaHareketiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateBankaHareketiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BankaHareketleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Banka hareketi bulunamadı.");

        entity.Tarih = request.Dto.Tarih;
        entity.Aciklama = request.Dto.Aciklama;
        entity.ReferansNo = request.Dto.ReferansNo;
        entity.Tutar = request.Dto.Tutar;
        entity.Durum = request.Dto.Durum;
        entity.EslestirmeId = request.Dto.EslestirmeId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteBankaHareketiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteBankaHareketiCommandHandler : IRequestHandler<DeleteBankaHareketiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteBankaHareketiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBankaHareketiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BankaHareketleri
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Banka hareketi bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
