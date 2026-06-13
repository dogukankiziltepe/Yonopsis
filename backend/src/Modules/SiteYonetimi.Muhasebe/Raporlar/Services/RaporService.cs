using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Raporlar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Raporlar.Services;

/// <summary>
/// Defter ve ekstre raporları için ortak hesap hareketi çıkarımı. Yalnızca
/// onaylı (entegre) fişler dikkate alınır; yürüyen bakiye hesaplanır.
/// </summary>
public interface IRaporService
{
    Task<Result<DefterDto>> HesapDefteriAsync(
        Guid siteId, Guid hesapId, DateOnly? from, DateOnly? to, bool altHesaplariDahilEt, CancellationToken ct);
}

public class RaporService : IRaporService
{
    private readonly SharedTenantDbContext _db;

    public RaporService(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DefterDto>> HesapDefteriAsync(
        Guid siteId, Guid hesapId, DateOnly? from, DateOnly? to, bool altHesaplariDahilEt, CancellationToken ct)
    {
        var hesap = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == siteId && h.Id == hesapId, ct);
        if (hesap is null)
            return Result<DefterDto>.Failure("Hesap bulunamadı.");

        // Onaylı fiş satırları + hesap filtresi (alt hesaplar dahil ise kod öneki ile).
        var kod = hesap.HesapKodu;
        var taban =
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            where d.SiteId == siteId && f.Durum == FisDurumu.Onaylandi
            select new { d, f };

        taban = altHesaplariDahilEt
            ? taban.Where(x => x.d.HesapKodu == kod || x.d.HesapKodu.StartsWith(kod + "."))
            : taban.Where(x => x.d.HesapId == hesapId);

        // Açılış bakiyesi (from'dan önceki net hareket).
        decimal acilis = 0;
        if (from is not null)
        {
            var oncesi = taban.Where(x => x.f.FisTarihi < from);
            var borcOnce = await oncesi.SumAsync(x => (decimal?)x.d.BorcTutar, ct) ?? 0;
            var alacakOnce = await oncesi.SumAsync(x => (decimal?)x.d.AlacakTutar, ct) ?? 0;
            acilis = borcOnce - alacakOnce;
        }

        var aralik = taban;
        if (from is not null) aralik = aralik.Where(x => x.f.FisTarihi >= from);
        if (to is not null) aralik = aralik.Where(x => x.f.FisTarihi <= to);

        var ham = await aralik
            .OrderBy(x => x.f.FisTarihi).ThenBy(x => x.f.YevmiyeNo).ThenBy(x => x.d.SiraNo)
            .Select(x => new
            {
                x.f.FisTarihi, x.f.FisNo, x.f.YevmiyeNo,
                x.d.HesapKodu, x.d.Aciklama, x.d.BorcTutar, x.d.AlacakTutar
            })
            .ToListAsync(ct);

        // Alt hesaplar dahil ise satırlardaki hesap adlarını çözmek için sözlük.
        var hesapAdlari = altHesaplariDahilEt
            ? await _db.HesapPlani.Where(h => h.SiteId == siteId &&
                  (h.HesapKodu == kod || h.HesapKodu.StartsWith(kod + ".")))
                .ToDictionaryAsync(h => h.HesapKodu, h => h.HesapAdi, ct)
            : new Dictionary<string, string> { [hesap.HesapKodu] = hesap.HesapAdi };

        var satirlar = new List<HareketSatirDto>();
        var yuruyen = acilis;
        decimal toplamBorc = 0, toplamAlacak = 0;
        foreach (var r in ham)
        {
            yuruyen += r.BorcTutar - r.AlacakTutar;
            toplamBorc += r.BorcTutar;
            toplamAlacak += r.AlacakTutar;
            satirlar.Add(new HareketSatirDto(
                r.FisTarihi, r.FisNo, r.YevmiyeNo, r.HesapKodu,
                hesapAdlari.GetValueOrDefault(r.HesapKodu, string.Empty),
                r.Aciklama, r.BorcTutar, r.AlacakTutar, yuruyen));
        }

        var dto = new DefterDto(
            hesap.Id, hesap.HesapKodu, hesap.HesapAdi,
            acilis, toplamBorc, toplamAlacak, acilis + toplamBorc - toplamAlacak, satirlar);

        return Result<DefterDto>.Success(dto);
    }
}
