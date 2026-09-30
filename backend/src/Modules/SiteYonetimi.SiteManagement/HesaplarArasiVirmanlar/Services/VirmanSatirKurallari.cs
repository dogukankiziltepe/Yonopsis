using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Services;

/// <summary>
/// Virman satırı ortak kuralları: tutar doğrulaması ve HesapTuru'na göre polimorfik
/// HesapId'nin site içinde var olduğunun kontrolü + görüntü adının çözülmesi.
/// Hem fiş ekranı (Hesaplar Arası Virman) hem satır ekranı (Virman Fişi Detayları) kullanır.
/// </summary>
internal static class VirmanSatirKurallari
{
    public static string? TutarHatasi(decimal borc, decimal alacak, string onEk = "")
    {
        if (borc < 0 || alacak < 0) return $"{onEk}Tutar negatif olamaz.";
        if (borc == 0 && alacak == 0) return $"{onEk}Borç veya alacak tutarı girilmelidir.";
        if (borc > 0 && alacak > 0) return $"{onEk}Borç ve alacak aynı anda girilemez.";
        return null;
    }

    /// <summary>Verilen hesapların adlarını döner; bulunamayan varsa hata mesajı döner.</summary>
    public static async Task<(Dictionary<(VirmanHesapTuru, Guid), string>? Adlar, string? Error)> HesaplariCozAsync(
        SharedTenantDbContext db, MasterDbContext masterDb, Guid siteId,
        IEnumerable<(VirmanHesapTuru Tur, Guid Id)> hesaplar, CancellationToken ct)
    {
        var liste = hesaplar.Distinct().ToList();
        if (liste.Any(h => h.Id == Guid.Empty)) return (null, "Hesap seçilmelidir.");

        var adlar = new Dictionary<(VirmanHesapTuru, Guid), string>();

        async Task Ekle(VirmanHesapTuru tur, Func<List<Guid>, Task<Dictionary<Guid, string>>> sorgu)
        {
            var ids = liste.Where(h => h.Tur == tur).Select(h => h.Id).ToList();
            if (ids.Count == 0) return;
            foreach (var (id, ad) in await sorgu(ids)) adlar[(tur, id)] = ad;
        }

        await Ekle(VirmanHesapTuru.Kisi, async ids =>
        {
            var siteKisileri = await masterDb.UserSites
                .Where(us => us.SiteId == siteId && ids.Contains(us.UserId))
                .Select(us => us.UserId).Distinct().ToListAsync(ct);
            return await masterDb.Users.Where(u => siteKisileri.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", ct);
        });
        await Ekle(VirmanHesapTuru.Cari, ids => db.HesapPlani
            .Where(h => h.SiteId == siteId && h.CariTuru != null && ids.Contains(h.Id))
            .ToDictionaryAsync(h => h.Id, h => h.HesapAdi, ct));
        await Ekle(VirmanHesapTuru.Banka, ids => db.KasaBanka
            .Where(k => k.SiteId == siteId && ids.Contains(k.Id))
            .ToDictionaryAsync(k => k.Id, k => k.Name, ct));
        await Ekle(VirmanHesapTuru.Gider, ids => db.GiderTanimlari
            .Where(g => g.SiteId == siteId && ids.Contains(g.Id))
            .ToDictionaryAsync(g => g.Id, g => g.Name, ct));
        await Ekle(VirmanHesapTuru.Personel, ids => db.Personeller
            .Where(p => p.SiteId == siteId && ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name, ct));

        return adlar.Count == liste.Count ? (adlar, null) : (null, "Seçilen hesaplardan biri bulunamadı.");
    }

    public static async Task<string?> DaireVeKategoriHatasiAsync(
        SharedTenantDbContext db, Guid siteId, IEnumerable<Guid?> unitIds, IEnumerable<Guid?> tanimIds, CancellationToken ct)
    {
        var units = unitIds.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        if (units.Count > 0 && await db.Units.CountAsync(u => u.SiteId == siteId && units.Contains(u.Id), ct) != units.Count)
            return "Seçilen dairelerden biri bulunamadı.";

        var tanimlar = tanimIds.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        if (tanimlar.Count > 0 && await db.GelirTanimlari.CountAsync(g => g.SiteId == siteId && tanimlar.Contains(g.Id), ct) != tanimlar.Count)
            return "Seçilen kategorilerden biri bulunamadı.";

        return null;
    }
}
