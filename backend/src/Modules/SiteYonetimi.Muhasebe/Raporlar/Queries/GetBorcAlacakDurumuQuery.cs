using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Raporlar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Raporlar.Queries;

/// <summary>Tüm cari hesapların özet bakiyeleri (borç/alacak durumu).</summary>
public record GetBorcAlacakDurumuQuery(Guid SiteId, CariTuru? CariTuru = null)
    : IRequest<Result<List<CariBakiyeDto>>>;

public class GetBorcAlacakDurumuQueryHandler
    : IRequestHandler<GetBorcAlacakDurumuQuery, Result<List<CariBakiyeDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetBorcAlacakDurumuQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<CariBakiyeDto>>> Handle(GetBorcAlacakDurumuQuery request, CancellationToken cancellationToken)
    {
        // Cari hesaplar
        var cariQuery = _db.HesapPlani
            .Where(h => h.SiteId == request.SiteId && h.HesapTipi == HesapTipi.Cari);
        if (request.CariTuru is not null)
            cariQuery = cariQuery.Where(h => h.CariTuru == request.CariTuru);

        var cariler = await cariQuery
            .Select(h => new { h.Id, h.HesapKodu, h.HesapAdi, h.CariTuru })
            .ToListAsync(cancellationToken);

        if (cariler.Count == 0)
            return Result<List<CariBakiyeDto>>.Success(new List<CariBakiyeDto>());

        var cariIdler = cariler.Select(c => c.Id).ToList();

        // Onaylı fiş satırlarından hesap bazlı toplamlar
        var toplamlar = await (
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            where d.SiteId == request.SiteId && f.Durum == FisDurumu.Onaylandi && cariIdler.Contains(d.HesapId)
            group d by d.HesapId into g
            select new { HesapId = g.Key, Borc = g.Sum(x => x.BorcTutar), Alacak = g.Sum(x => x.AlacakTutar) }
        ).ToDictionaryAsync(x => x.HesapId, cancellationToken);

        var liste = cariler
            .OrderBy(c => c.HesapKodu, StringComparer.Ordinal)
            .Select(c =>
            {
                var t = toplamlar.GetValueOrDefault(c.Id);
                var borc = t?.Borc ?? 0;
                var alacak = t?.Alacak ?? 0;
                return new CariBakiyeDto(c.Id, c.HesapKodu, c.HesapAdi, c.CariTuru, borc, alacak, borc - alacak);
            })
            .ToList();

        return Result<List<CariBakiyeDto>>.Success(liste);
    }
}
