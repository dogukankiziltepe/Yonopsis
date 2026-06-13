using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Services;

/// <summary>
/// Fiş oluşturma/güncelleme sırasında paylaşılan iş kuralları: dönem çözümleme,
/// satır doğrulama (denge/hesap kuralları) ve fiş no üretimi.
/// </summary>
public interface IFisService
{
    /// <summary>İlgili yıl için açık dönemi bulur; yoksa açık dönem oluşturur. Kapalıysa hata.</summary>
    Task<Result<MuhasebeDonem>> ResolveAcikDonemAsync(Guid siteId, int yil, CancellationToken ct);

    /// <summary>Satırları doğrular ve detay entity'lerine dönüştürür (SiraNo + HesapKodu set eder).</summary>
    Task<Result<List<MuhasebeFisiDetay>>> BuildDetaylarAsync(
        Guid siteId, Guid fisId, IReadOnlyList<FisDetaySatirDto> satirlar, CancellationToken ct);

    /// <summary>İlgili yıl için sıradaki fiş numarasını üretir (örn. "2026-0000123").</summary>
    Task<string> GenerateFisNoAsync(Guid siteId, int yil, CancellationToken ct);
}

public class FisService : IFisService
{
    private readonly SharedTenantDbContext _db;

    public FisService(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<MuhasebeDonem>> ResolveAcikDonemAsync(Guid siteId, int yil, CancellationToken ct)
    {
        var donem = await _db.MuhasebeDonemler
            .FirstOrDefaultAsync(d => d.SiteId == siteId && d.Yil == yil, ct);

        if (donem is null)
        {
            donem = new MuhasebeDonem
            {
                SiteId = siteId,
                Yil = yil,
                Ad = $"{yil} Mali Yılı",
                BaslangicTarihi = new DateOnly(yil, 1, 1),
                BitisTarihi = new DateOnly(yil, 12, 31),
                Durum = DonemDurumu.Acik,
                SonYevmiyeNo = 0
            };
            _db.MuhasebeDonemler.Add(donem);
            return Result<MuhasebeDonem>.Success(donem);
        }

        if (donem.Durum == DonemDurumu.Kapali)
            return Result<MuhasebeDonem>.Failure($"{yil} dönemi kapalı; fiş girilemez.");

        return Result<MuhasebeDonem>.Success(donem);
    }

    public async Task<Result<List<MuhasebeFisiDetay>>> BuildDetaylarAsync(
        Guid siteId, Guid fisId, IReadOnlyList<FisDetaySatirDto> satirlar, CancellationToken ct)
    {
        if (satirlar.Count == 0)
            return Result<List<MuhasebeFisiDetay>>.Success(new List<MuhasebeFisiDetay>());

        var hesapIdler = satirlar.Select(s => s.HesapId).Distinct().ToList();
        var hesaplar = await _db.HesapPlani
            .Where(h => h.SiteId == siteId && hesapIdler.Contains(h.Id))
            .ToDictionaryAsync(h => h.Id, ct);

        var detaylar = new List<MuhasebeFisiDetay>();
        var sira = 1;
        foreach (var s in satirlar)
        {
            if (!hesaplar.TryGetValue(s.HesapId, out var hesap))
                return Result<List<MuhasebeFisiDetay>>.Failure($"Geçersiz hesap (satır {sira}).");
            if (!hesap.AktifMi)
                return Result<List<MuhasebeFisiDetay>>.Failure($"Pasif hesaba fiş kesilemez: {hesap.HesapKodu} (satır {sira}).");
            if (!hesap.FisKesilebilirMi)
                return Result<List<MuhasebeFisiDetay>>.Failure($"Bu hesaba fiş kesilemez: {hesap.HesapKodu} (satır {sira}).");

            var borc = s.BorcTutar;
            var alacak = s.AlacakTutar;
            if (borc < 0 || alacak < 0)
                return Result<List<MuhasebeFisiDetay>>.Failure($"Tutar negatif olamaz (satır {sira}).");
            var borcDolu = borc > 0;
            var alacakDolu = alacak > 0;
            if (borcDolu == alacakDolu)
                return Result<List<MuhasebeFisiDetay>>.Failure(
                    $"Her satırda yalnızca borç veya yalnızca alacak dolu olmalı (satır {sira}).");

            detaylar.Add(new MuhasebeFisiDetay
            {
                FisId = fisId,
                SiteId = siteId,
                SiraNo = sira,
                HesapId = hesap.Id,
                HesapKodu = hesap.HesapKodu,
                BorcTutar = borc,
                AlacakTutar = alacak,
                Aciklama = s.Aciklama?.Trim(),
                BelgeNo = s.BelgeNo?.Trim()
            });
            sira++;
        }

        return Result<List<MuhasebeFisiDetay>>.Success(detaylar);
    }

    public async Task<string> GenerateFisNoAsync(Guid siteId, int yil, CancellationToken ct)
    {
        var onek = $"{yil}-";
        var kodlar = await _db.MuhasebeFisleri
            .Where(f => f.SiteId == siteId && f.FisNo.StartsWith(onek))
            .Select(f => f.FisNo)
            .ToListAsync(ct);

        var son = 0;
        foreach (var kod in kodlar)
        {
            if (int.TryParse(kod[onek.Length..], out var n) && n > son)
                son = n;
        }
        return $"{onek}{son + 1:0000000}";
    }
}
