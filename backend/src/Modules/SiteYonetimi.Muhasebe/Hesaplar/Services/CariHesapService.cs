using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Hesaplar.Services;

/// <summary>
/// Cari hesap (kiracı/ev sahibi/tedarikçi/personel) oluşturma mantığını
/// tek noktada toplar. Hem manuel ekleme (CreateCariHesapCommand) hem de
/// otomatik oluşturma (Faz 5: PersonCreated handler) bu servisi kullanır.
/// </summary>
public interface ICariHesapService
{
    /// <summary>
    /// İlgili ana hesap altında bir cari hesap açar. PersonId verilmişse
    /// idempotenttir: aynı kişi için ikinci kez cari açılmaz, mevcut Id döner.
    /// SaveChanges çağırır.
    /// </summary>
    Task<Result<Guid>> EnsureCariHesapAsync(
        Guid siteId,
        CariTuru cariTuru,
        string hesapAdi,
        Guid? personId,
        string? aciklama,
        CancellationToken ct);
}

public class CariHesapService : ICariHesapService
{
    private readonly SharedTenantDbContext _db;

    public CariHesapService(SharedTenantDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Cari türüne göre varsayılan ana hesap kodu. (Faz 6'da MuhasebeParametre
    /// üzerinden override edilebilir olacak.)
    /// </summary>
    private static string AnaHesapKodu(CariTuru tur) => tur switch
    {
        CariTuru.Kiraci => "120",
        CariTuru.EvSahibi => "120",
        CariTuru.Tedarikci => "320",
        CariTuru.Personel => "335",
        _ => "120"
    };

    public async Task<Result<Guid>> EnsureCariHesapAsync(
        Guid siteId,
        CariTuru cariTuru,
        string hesapAdi,
        Guid? personId,
        string? aciklama,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(hesapAdi))
            return Result<Guid>.Failure("Cari hesap adı zorunludur.");

        // İdempotentlik: aynı kişi + tür için zaten cari varsa onu döndür.
        if (personId is not null)
        {
            var mevcut = await _db.HesapPlani
                .FirstOrDefaultAsync(h => h.SiteId == siteId
                    && h.PersonId == personId
                    && h.CariTuru == cariTuru, ct);
            if (mevcut is not null)
                return Result<Guid>.Success(mevcut.Id);
        }

        var anaKodu = AnaHesapKodu(cariTuru);
        var ana = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == anaKodu, ct);
        if (ana is null)
            return Result<Guid>.Failure($"Ana hesap ({anaKodu}) bulunamadı. Önce hesap planı seed edilmeli.");

        // Ara grup hesabı (örn. 120.01) — yoksa oluştur.
        var araKodu = $"{anaKodu}.01";
        var ara = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == araKodu, ct);
        if (ara is null)
        {
            ara = new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = araKodu,
                HesapAdi = $"{ana.HesapAdi} - Cari",
                HesapTipi = HesapTipi.Tekduzen,
                HesapKategorisi = ana.HesapKategorisi,
                NormalBakiye = ana.NormalBakiye,
                Seviye = ana.Seviye + 1,
                ParentId = ana.Id,
                FisKesilebilirMi = false,
                AktifMi = true
            };
            _db.HesapPlani.Add(ara);
            // Ana hesap artık yaprak değil.
            if (ana.FisKesilebilirMi)
            {
                ana.FisKesilebilirMi = false;
                ana.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Sıradaki cari numarasını bul (araKodu + "." öneki altındaki en büyük sıra + 1).
        var onek = araKodu + ".";
        var mevcutKodlar = await _db.HesapPlani
            .Where(h => h.SiteId == siteId && h.HesapKodu.StartsWith(onek))
            .Select(h => h.HesapKodu)
            .ToListAsync(ct);

        var sonSira = 0;
        foreach (var kod in mevcutKodlar)
        {
            var sonSegment = kod[onek.Length..];
            if (int.TryParse(sonSegment, out var n) && n > sonSira)
                sonSira = n;
        }
        var yeniKod = $"{onek}{sonSira + 1:0000}";

        var cari = new HesapPlani
        {
            SiteId = siteId,
            HesapKodu = yeniKod,
            HesapAdi = hesapAdi.Trim(),
            HesapTipi = HesapTipi.Cari,
            HesapKategorisi = ana.HesapKategorisi,
            NormalBakiye = ana.NormalBakiye,
            Seviye = ara.Seviye + 1,
            ParentId = ara.Id,
            FisKesilebilirMi = true,
            CariTuru = cariTuru,
            PersonId = personId,
            Aciklama = aciklama,
            AktifMi = true
        };

        _db.HesapPlani.Add(cari);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(cari.Id);
    }
}
