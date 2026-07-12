using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OtomatikBildirimler.DTOs;

namespace SiteYonetimi.SiteManagement.OtomatikBildirimler.Commands;

public record UpsertOtomatikBildirimCommand(Guid SiteId, UpsertOtomatikBildirimDto Dto) : IRequest<Result<Guid>>;
public class UpsertOtomatikBildirimCommandHandler : IRequestHandler<UpsertOtomatikBildirimCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public UpsertOtomatikBildirimCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(UpsertOtomatikBildirimCommand request, CancellationToken ct)
    {
        var entity = await _db.OtomatikBildirimler.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.SiteId == request.SiteId && x.OlayTipi == request.Dto.OlayTipi, ct);
        if (entity is null)
        {
            entity = new OtomatikBildirim { SiteId = request.SiteId };
            _db.OtomatikBildirimler.Add(entity);
        }
        entity.OlayTipi = request.Dto.OlayTipi; entity.EpostaAktif = request.Dto.EpostaAktif;
        entity.SmsAktif = request.Dto.SmsAktif; entity.MobilAktif = request.Dto.MobilAktif;
        entity.EpostaSablonuId = request.Dto.EpostaSablonuId; entity.SmsSablonuId = request.Dto.SmsSablonuId;
        entity.MobilSablonuId = request.Dto.MobilSablonuId; entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}
