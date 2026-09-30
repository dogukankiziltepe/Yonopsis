using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Services;

/// <summary>
/// İcra takibi ekranlarının ortak hesapları: takibe alınabilecek açık borç makbuzları,
/// takip/dosya bazında canlı kalan tutar ve borçlu iletişim bilgileri.
/// </summary>
internal static class IcraHesaplari
{
    /// <summary>Süreci devam eden takiplere bağlı makbuzlar yeniden takibe alınamaz.</summary>
    public static readonly TakipDurumu[] AktifTakipDurumlari =
        { TakipDurumu.Takipte, TakipDurumu.IcrayaVerilecek, TakipDurumu.IcrayaVerildi };

    /// <summary>Filtreye uyan, vadesi geçmiş, kalanı olan ve aktif bir takipte olmayan borç makbuzları.</summary>
    public static IQueryable<BorcMakbuzu> AcikMakbuzlar(SharedTenantDbContext db, Guid siteId, TakipAdayFiltreDto f)
    {
        var sinir = DateTime.UtcNow.Date.AddDays(-Math.Max(0, f.GunSayisi));
        var takiptekiler = db.IcraTakipEvraklari
            .Where(e => AktifTakipDurumlari.Contains(e.Takip.Durum))
            .Select(e => e.BorcMakbuzuId);

        var q = db.BorcMakbuzlari.Where(b =>
            b.SiteId == siteId && b.BorcluUserId != null && b.UnitId != null &&
            b.SonOdemeTarihi != null && b.SonOdemeTarihi <= sinir &&
            b.Tutar + b.GecikmeTutari - b.OdenenTutar > 0 &&
            !takiptekiler.Contains(b.Id));

        if (f.UnitId.HasValue) q = q.Where(b => b.UnitId == f.UnitId);
        else if (f.BuildingId.HasValue) q = q.Where(b => b.Unit!.BuildingId == f.BuildingId);
        if (f.GelirTanimiId.HasValue) q = q.Where(b => b.GelirTanimiId == f.GelirTanimiId);
        return q;
    }

    /// <summary>Takip Id → bağlı makbuzların güncel kalan toplamı (silinmiş makbuzlar sayılmaz).</summary>
    public static Task<Dictionary<Guid, decimal>> TakipKalanlariAsync(SharedTenantDbContext db, List<Guid> takipIds, CancellationToken ct) =>
        db.IcraTakipEvraklari.Where(e => takipIds.Contains(e.TakipId))
            .Join(db.BorcMakbuzlari, e => e.BorcMakbuzuId, b => b.Id, (e, b) => new { e.TakipId, Kalan = b.Tutar + b.GecikmeTutari - b.OdenenTutar })
            .GroupBy(x => x.TakipId)
            .Select(g => new { g.Key, Toplam = g.Sum(x => x.Kalan) })
            .ToDictionaryAsync(x => x.Key, x => x.Toplam, ct);

    /// <summary>Dosya Id → bağlı makbuzların güncel kalan toplamı ve evrak sayısı.</summary>
    public static async Task<Dictionary<Guid, (decimal Kalan, int Sayi)>> DosyaKalanlariAsync(SharedTenantDbContext db, List<Guid> dosyaIds, CancellationToken ct)
    {
        var rows = await db.IcraDosyasiEvraklari.Where(e => dosyaIds.Contains(e.DosyaId))
            .Join(db.BorcMakbuzlari, e => e.BorcMakbuzuId, b => b.Id, (e, b) => new { e.DosyaId, Kalan = b.Tutar + b.GecikmeTutari - b.OdenenTutar })
            .GroupBy(x => x.DosyaId)
            .Select(g => new { g.Key, Toplam = g.Sum(x => x.Kalan), Sayi = g.Count() })
            .ToListAsync(ct);
        return rows.ToDictionary(x => x.Key, x => (x.Toplam, x.Sayi));
    }

    public static Task<List<IcraEvrakDto>> EvraklarAsync(SharedTenantDbContext db, IQueryable<Guid> makbuzIds, CancellationToken ct) =>
        db.BorcMakbuzlari.Where(b => makbuzIds.Contains(b.Id))
            .OrderByDescending(b => b.SonOdemeTarihi)
            .Select(b => new IcraEvrakDto(
                b.Id, b.EvrakNo, b.SonOdemeTarihi, b.Unit != null ? b.Unit.DoorNumber : "",
                b.GelirTanimi != null ? b.GelirTanimi.Name : null,
                b.Tutar, b.GecikmeTutari, b.OdenenTutar, b.Tutar + b.GecikmeTutari - b.OdenenTutar))
            .ToListAsync(ct);

    public record Iletisim(string Ad, string? Telefon, string? Eposta, string? Adres);

    /// <summary>Borçluların güncel ad, telefon, e-posta ve (site bazlı) adres bilgileri.</summary>
    public static async Task<Dictionary<Guid, Iletisim>> IletisimAsync(MasterDbContext masterDb, Guid siteId, IEnumerable<Guid> userIds, CancellationToken ct)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0) return new();
        var adresler = await masterDb.UserSites
            .Where(us => us.SiteId == siteId && ids.Contains(us.UserId) && us.Address != null && us.Address != "")
            .Select(us => new { us.UserId, us.Address })
            .ToListAsync(ct);
        var users = await masterDb.Users.Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.PhoneNumber, u.Email })
            .ToListAsync(ct);
        return users.ToDictionary(u => u.Id, u => new Iletisim(
            $"{u.FirstName} {u.LastName}", u.PhoneNumber, u.Email,
            adresler.FirstOrDefault(a => a.UserId == u.Id)?.Address));
    }
}
