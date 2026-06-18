using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.Services;

/// <summary>Dönem sonu hesaplamaları için ortak bakiye çıkarımı.</summary>
public interface IDonemSonuService
{
    /// <summary>İlgili dönemdeki onaylı fişlerden hesap bazlı net bakiyeleri (net != 0) döner.</summary>
    Task<List<DonemHesapBakiye>> HesapBakiyeleriAsync(Guid siteId, Guid donemId, CancellationToken ct);
}

public class DonemSonuService : IDonemSonuService
{
    private readonly SharedTenantDbContext _db;

    public DonemSonuService(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<List<DonemHesapBakiye>> HesapBakiyeleriAsync(Guid siteId, Guid donemId, CancellationToken ct)
    {
        var gruplar = await (
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            join h in _db.HesapPlani on d.HesapId equals h.Id
            where d.SiteId == siteId && f.DonemId == donemId && f.Durum == FisDurumu.Onaylandi
            group new { d.BorcTutar, d.AlacakTutar } by new { d.HesapId, d.HesapKodu, h.HesapAdi, h.HesapKategorisi } into g
            select new
            {
                g.Key.HesapId,
                g.Key.HesapKodu,
                g.Key.HesapAdi,
                g.Key.HesapKategorisi,
                Net = g.Sum(x => x.BorcTutar) - g.Sum(x => x.AlacakTutar)
            }).ToListAsync(ct);

        return gruplar
            .Where(g => g.Net != 0)
            .OrderBy(g => g.HesapKodu, StringComparer.Ordinal)
            .Select(g => new DonemHesapBakiye(g.HesapId, g.HesapKodu, g.HesapAdi, g.HesapKategorisi, g.Net))
            .ToList();
    }
}
