using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Seed;

/// <summary>
/// Muhasebe modülü için tenant (site) bazlı seed iskeleti.
/// Yeni bir site oluşturulduğunda varsayılan (site yönetimine özel,
/// sadeleştirilmiş TDHP) hesap planı ile mali dönem seed edilir.
///
/// NOT (Faz 1): Bu seeder iskelettir. Site oluşturma akışına bağlanması
/// (CreateSiteCommand içinde çağrılması) ileriki fazlarda yapılacaktır.
/// Çağrı idempotenttir: ilgili site için hesap planı zaten varsa atlanır.
/// </summary>
public static class MuhasebeSeeder
{
    /// <summary>
    /// Bir site için varsayılan hesap planını ve açık mali dönemi seed eder.
    /// SaveChanges çağırır.
    /// </summary>
    public static async Task SeedForSiteAsync(SharedTenantDbContext db, Guid siteId, int? yil = null)
    {
        await SeedHesapPlaniAsync(db, siteId);
        await SeedAcikDonemAsync(db, siteId, yil ?? DateTime.UtcNow.Year);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Varsayılan hesap planını seed eder. Hesaplar arası hiyerarşi (ParentId),
    /// seviye ve "fiş kesilebilir" (yaprak hesap) bilgisi otomatik hesaplanır.
    /// </summary>
    public static async Task SeedHesapPlaniAsync(SharedTenantDbContext db, Guid siteId)
    {
        var zatenVar = await db.HesapPlani.IgnoreQueryFilters()
            .AnyAsync(h => h.SiteId == siteId);
        if (zatenVar) return;

        // Kod, Ad, Kategori, Normal bakiye yönü — hiyerarşi koddan türetilir.
        var tanimlar = VarsayilanPlan();

        // Kod -> tanım sözlüğü (parent çözümü için)
        var kodSet = tanimlar.Select(t => t.Kod).ToHashSet();

        // Bir kodun en uzun (gerçek) önek-üst kodunu bul (örn. "770.01" -> "770", "100" -> "10").
        string? ParentKodBul(string kod)
        {
            string? enUygun = null;
            foreach (var aday in kodSet)
            {
                if (aday == kod) continue;
                if (kod.StartsWith(aday) && (enUygun is null || aday.Length > enUygun.Length))
                    enUygun = aday;
            }
            return enUygun;
        }

        // Yaprak (alt hesabı olmayan) hesaplara fiş kesilebilir.
        bool YaprakMi(string kod) => !kodSet.Any(a => a != kod && a.StartsWith(kod));

        // Kod -> oluşturulan entity (ParentId ataması için)
        var olusturulan = new Dictionary<string, HesapPlani>();

        // Seviye = üst zincir uzunluğu + 1; üstler önce işlensin diye koda göre sırala.
        foreach (var t in tanimlar.OrderBy(t => t.Kod, StringComparer.Ordinal))
        {
            var parentKod = ParentKodBul(t.Kod);
            var seviye = 1;
            var p = parentKod;
            while (p is not null)
            {
                seviye++;
                p = ParentKodBul(p);
            }

            var hesap = new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = t.Kod,
                HesapAdi = t.Ad,
                HesapTipi = HesapTipi.Tekduzen,
                HesapKategorisi = t.Kategori,
                NormalBakiye = t.Bakiye,
                Seviye = seviye,
                ParentId = parentKod is not null && olusturulan.TryGetValue(parentKod, out var pe) ? pe.Id : null,
                FisKesilebilirMi = YaprakMi(t.Kod),
                AktifMi = true
            };

            olusturulan[t.Kod] = hesap;
            db.HesapPlani.Add(hesap);
        }
    }

    /// <summary>
    /// İlgili yıl için açık mali dönem yoksa oluşturur.
    /// </summary>
    public static async Task SeedAcikDonemAsync(SharedTenantDbContext db, Guid siteId, int yil)
    {
        var zatenVar = await db.MuhasebeDonemler.IgnoreQueryFilters()
            .AnyAsync(d => d.SiteId == siteId && d.Yil == yil);
        if (zatenVar) return;

        db.MuhasebeDonemler.Add(new MuhasebeDonem
        {
            SiteId = siteId,
            Yil = yil,
            Ad = $"{yil} Mali Yılı",
            BaslangicTarihi = new DateOnly(yil, 1, 1),
            BitisTarihi = new DateOnly(yil, 12, 31),
            Durum = DonemDurumu.Acik,
            SonYevmiyeNo = 0
        });
    }

    private record HesapTanim(string Kod, string Ad, HesapKategorisi Kategori, NormalBakiye Bakiye);

    /// <summary>
    /// Site yönetimine özel, sadeleştirilmiş varsayılan hesap planı (bkz. görev §11).
    /// Mali müşavir ile gözden geçirilip tenant bazında özelleştirilebilir.
    /// </summary>
    private static List<HesapTanim> VarsayilanPlan() => new()
    {
        // 1 — DÖNEN VARLIKLAR
        new("1",   "DÖNEN VARLIKLAR",                 HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("10",  "HAZIR DEĞERLER",                  HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("100", "KASA",                            HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("102", "BANKALAR",                        HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("12",  "TİCARİ ALACAKLAR",                HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("120", "ALICILAR (AİDAT ALACAKLARI)",     HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("126", "VERİLEN DEPOZİTO VE TEMİNATLAR",  HesapKategorisi.Aktif,  NormalBakiye.Borc),

        // 3 — KISA VADELİ YABANCI KAYNAKLAR
        new("3",   "KISA VADELİ YABANCI KAYNAKLAR",   HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("32",  "TİCARİ BORÇLAR",                  HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("320", "SATICILAR (TEDARİKÇİLER)",        HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("326", "ALINAN DEPOZİTO VE TEMİNATLAR",   HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("335", "PERSONELE BORÇLAR",               HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("360", "ÖDENECEK VERGİ VE FONLAR",        HesapKategorisi.Pasif,  NormalBakiye.Alacak),

        // 6 — GELİR TABLOSU HESAPLARI
        new("6",   "GELİR TABLOSU HESAPLARI",         HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("60",  "BRÜT SATIŞLAR / GELİRLER",        HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("600", "AİDAT VE ORTAK GİDER GELİRLERİ",  HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("642", "GECİKME FAİZİ GELİRLERİ",         HesapKategorisi.Gelir,  NormalBakiye.Alacak),

        // 7 — MALİYET / GİDER HESAPLARI
        new("7",      "MALİYET HESAPLARI",            HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("77",     "GENEL YÖNETİM GİDERLERİ",      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770",    "GENEL YÖNETİM GİDERLERİ",      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.01", "Elektrik",                     HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.02", "Su",                           HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.03", "Doğalgaz",                     HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.04", "Asansör Bakım",                HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.05", "Temizlik",                     HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.06", "Güvenlik",                     HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.07", "Peyzaj/Bahçe",                 HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.08", "Sigorta",                      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.09", "Personel Maaş",                HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.10", "Ortak Alan Bakım/Onarım",      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.99", "Diğer Giderler",               HesapKategorisi.Gider,  NormalBakiye.Borc),
    };
}
