using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.Queries;

/// <summary>En güncel açık dönemi döner (yoksa null).</summary>
public record GetAktifDonemQuery(Guid SiteId) : IRequest<Result<DonemDto?>>;

public class GetAktifDonemQueryHandler : IRequestHandler<GetAktifDonemQuery, Result<DonemDto?>>
{
    private readonly SharedTenantDbContext _db;

    public GetAktifDonemQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DonemDto?>> Handle(GetAktifDonemQuery request, CancellationToken cancellationToken)
    {
        var donem = await _db.MuhasebeDonemler
            .Where(d => d.SiteId == request.SiteId && d.Durum == DonemDurumu.Acik)
            .OrderByDescending(d => d.Yil)
            .Select(d => new DonemDto(d.Id, d.Yil, d.Ad, d.BaslangicTarihi, d.BitisTarihi, d.Durum, d.SonYevmiyeNo))
            .FirstOrDefaultAsync(cancellationToken);

        return Result<DonemDto?>.Success(donem);
    }
}
