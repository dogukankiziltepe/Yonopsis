using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Report.DTOs;

namespace SiteYonetimi.SiteManagement.Report.Services;

public class ReportService : IReportService
{
    private readonly SharedTenantDbContext _db;
    public ReportService(SharedTenantDbContext db) => _db = db;

    private static (DateTime From, DateTime To) ResolveYearRange(DateTime? from, DateTime? to)
    {
        var now = DateTime.UtcNow;
        var start = from ?? new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = to ?? now;
        return (start, end.Date.AddDays(1).AddTicks(-1));
    }

    private static (DateTime From, DateTime To) ResolveLastMonthsRange(DateTime? from, DateTime? to, int months)
    {
        var now = DateTime.UtcNow;
        var start = from ?? now.AddMonths(-months).Date;
        var end = to ?? now;
        return (start, end.Date.AddDays(1).AddTicks(-1));
    }

    public async Task<List<KasaBakiyeDto>> GetKasalarAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (rangeFrom, rangeTo) = ResolveYearRange(from, to);

        var kasalar = await _db.KasaBanka
            .Where(x => x.SiteId == siteId && x.IsActive)
            .ToListAsync(ct);

        var hareketler = await _db.BankaHareketleri
            .Where(x => x.SiteId == siteId && (all || x.Tarih <= rangeTo))
            .Select(x => new { x.KasaBankaId, x.Tarih, x.Tutar })
            .ToListAsync(ct);

        var result = new List<KasaBakiyeDto>();
        foreach (var kasa in kasalar)
        {
            var kasaHareketleri = hareketler.Where(h => h.KasaBankaId == kasa.Id).ToList();

            var devir = all ? 0m : kasaHareketleri.Where(h => h.Tarih < rangeFrom).Sum(h => h.Tutar);
            var donemHareketleri = all ? kasaHareketleri : kasaHareketleri.Where(h => h.Tarih >= rangeFrom && h.Tarih <= rangeTo).ToList();
            var giren = donemHareketleri.Where(h => h.Tutar > 0).Sum(h => h.Tutar);
            var cikan = donemHareketleri.Where(h => h.Tutar < 0).Sum(h => Math.Abs(h.Tutar));

            result.Add(new KasaBakiyeDto(kasa.Id, kasa.Name, kasa.Tip, devir, giren, cikan, devir + giren - cikan));
        }

        return result;
    }

    public async Task<List<IsTakibiOgesiDto>> GetIsTakibiAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (rangeFrom, rangeTo) = ResolveYearRange(from, to);

        var isEmirleriQuery = _db.IsEmirleri
            .Where(x => x.SiteId == siteId && x.Durum != IsEmriDurum.Tamamlandi && x.Durum != IsEmriDurum.Iptal);
        if (!all) isEmirleriQuery = isEmirleriQuery.Where(x => x.CreatedAt >= rangeFrom && x.CreatedAt <= rangeTo);

        var isEmirleri = await isEmirleriQuery
            .Select(x => new IsTakibiOgesiDto(x.Id, "IsEmri", x.Baslik, x.AtananKisiAdi, x.Oncelik.ToString(), x.Durum.ToString(), x.IslemBaslangic ?? x.CreatedAt))
            .ToListAsync(ct);

        var yapilacakIslerQuery = _db.YapilacakIsler
            .Where(x => x.SiteId == siteId && x.Durum != YapilacakIsDurum.Tamamlandi);
        if (!all) yapilacakIslerQuery = yapilacakIslerQuery.Where(x => x.CreatedAt >= rangeFrom && x.CreatedAt <= rangeTo);

        var yapilacakIsler = await yapilacakIslerQuery
            .Select(x => new IsTakibiOgesiDto(x.Id, "YapilacakIs", x.Baslik, x.AtananKisi, x.Oncelik.ToString(), x.Durum.ToString(), x.TamamlanmaTarihi ?? x.CreatedAt))
            .ToListAsync(ct);

        return isEmirleri.Concat(yapilacakIsler)
            .OrderByDescending(x => x.Tarih)
            .ToList();
    }

    public async Task<List<AidatTahsilatAyDto>> GetAidatTahsilatAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var minDonem = all ? (string?)null : (from ?? now.AddMonths(-11)).ToString("yyyy-MM");
        var maxDonem = all ? (string?)null : (to ?? now).ToString("yyyy-MM");

        var borclar = await _db.BorcMakbuzlari
            .Where(x => x.SiteId == siteId && x.Donem != null)
            .Select(x => new { x.Donem, x.Tutar, x.GecikmeTutari, x.OdenenTutar })
            .ToListAsync(ct);

        var filtered = all
            ? borclar
            : borclar.Where(x => string.Compare(x.Donem, minDonem, StringComparison.Ordinal) >= 0
                               && string.Compare(x.Donem, maxDonem, StringComparison.Ordinal) <= 0);

        return filtered
            .GroupBy(x => x.Donem!)
            .Select(g => new AidatTahsilatAyDto(
                g.Key,
                g.Sum(x => x.OdenenTutar),
                g.Sum(x => x.Tutar + x.GecikmeTutari - x.OdenenTutar)))
            .OrderBy(x => x.Donem, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<List<FinansalDurumNoktaDto>> GetFinansalDurumAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (rangeFrom, rangeTo) = ResolveLastMonthsRange(from, to, 3);

        var kasalar = await _db.KasaBanka
            .Where(x => x.SiteId == siteId && x.IsActive)
            .ToListAsync(ct);

        var hareketler = await _db.BankaHareketleri
            .Where(x => x.SiteId == siteId && x.Tarih <= rangeTo)
            .Select(x => new { x.KasaBankaId, x.Tarih, x.Tutar })
            .ToListAsync(ct);

        var startDay = all ? (hareketler.Count > 0 ? hareketler.Min(h => h.Tarih).Date : rangeFrom.Date) : rangeFrom.Date;
        var endDay = rangeTo.Date;

        var result = new List<FinansalDurumNoktaDto>();
        foreach (var kasa in kasalar)
        {
            var kasaHareketleri = hareketler.Where(h => h.KasaBankaId == kasa.Id).ToList();
            var runningBalance = kasaHareketleri.Where(h => h.Tarih.Date < startDay).Sum(h => h.Tutar);

            for (var day = startDay; day <= endDay; day = day.AddDays(1))
            {
                runningBalance += kasaHareketleri.Where(h => h.Tarih.Date == day).Sum(h => h.Tutar);
                result.Add(new FinansalDurumNoktaDto(day, kasa.Id, kasa.Name, runningBalance));
            }
        }

        return result;
    }

    public async Task<List<EvrakDto>> GetEvraklarAsync(Guid siteId, FaturaTipi tip, CancellationToken ct = default)
    {
        return await _db.Faturalar
            .Where(x => x.SiteId == siteId && x.Tip == tip)
            .OrderByDescending(x => x.IslemTarihi)
            .Take(10)
            .Select(x => new EvrakDto(x.Id, x.IslemTarihi, x.EvrakNo, x.CariAdi, x.ToplamTutar))
            .ToListAsync(ct);
    }

    public async Task<List<OdenecekFaturaDto>> GetOdenecekFaturalarAsync(Guid siteId, CancellationToken ct = default)
    {
        return await _db.Faturalar
            .Where(x => x.SiteId == siteId && x.Tip == FaturaTipi.Gider && x.OdemeDurumu == FaturaOdemeDurumu.Odenmedi)
            .OrderBy(x => x.SonOdemeTarihi)
            .Select(x => new OdenecekFaturaDto(x.Id, x.EvrakNo, x.CariAdi, x.ToplamTutar, x.SonOdemeTarihi))
            .ToListAsync(ct);
    }

    public async Task<List<DagilimDilimiDto>> GetDagilimAsync(Guid siteId, FaturaTipi tip, bool all, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (rangeFrom, rangeTo) = ResolveYearRange(from, to);

        var query = _db.Faturalar
            .Include(x => x.GelirTanimi)
            .Include(x => x.GiderTanimi)
            .Where(x => x.SiteId == siteId && x.Tip == tip);
        if (!all) query = query.Where(x => x.FaturaTarihi >= rangeFrom && x.FaturaTarihi <= rangeTo);

        var faturalar = await query
            .Select(x => new
            {
                Ad = tip == FaturaTipi.Gelir
                    ? (x.GelirTanimi != null ? x.GelirTanimi.Name : "Diğer")
                    : (x.GiderTanimi != null ? x.GiderTanimi.Name : "Diğer"),
                x.ToplamTutar
            })
            .ToListAsync(ct);

        var toplam = faturalar.Sum(x => x.ToplamTutar);
        return faturalar
            .GroupBy(x => x.Ad)
            .Select(g => new DagilimDilimiDto(
                g.Key,
                g.Sum(x => x.ToplamTutar),
                toplam > 0 ? (double)(g.Sum(x => x.ToplamTutar) / toplam * 100) : 0))
            .OrderByDescending(x => x.Tutar)
            .ToList();
    }

    public async Task<List<DuyuruOzetDto>> GetDuyurularAsync(Guid siteId, CancellationToken ct = default)
    {
        return await _db.Announcements
            .Where(x => x.SiteId == siteId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .Select(x => new DuyuruOzetDto(x.Id, x.Title, x.IsPinned, x.PublishDate, x.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<List<BankaHesabiDto>> GetBankaHesaplariAsync(Guid siteId, CancellationToken ct = default)
    {
        return await _db.KasaBanka
            .Where(x => x.SiteId == siteId && x.IsActive && x.Tip == KasaBankaTipi.BankHesabi)
            .Select(x => new BankaHesabiDto(x.Id, x.Name, x.BankaAdi, x.SubeAdi, x.HesapNo, x.IBAN))
            .ToListAsync(ct);
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(Guid siteId, CancellationToken ct = default)
    {
        var kasalar = await GetKasalarAsync(siteId, false, null, null, ct);
        var isTakibi = await GetIsTakibiAsync(siteId, false, null, null, ct);
        var aidatTahsilat = await GetAidatTahsilatAsync(siteId, false, null, null, ct);
        var finansalDurum = await GetFinansalDurumAsync(siteId, false, null, null, ct);
        var giderEvraklari = await GetEvraklarAsync(siteId, FaturaTipi.Gider, ct);
        var gelirEvraklari = await GetEvraklarAsync(siteId, FaturaTipi.Gelir, ct);
        var odenecekFaturalar = await GetOdenecekFaturalarAsync(siteId, ct);
        var giderDagilimi = await GetDagilimAsync(siteId, FaturaTipi.Gider, false, null, null, ct);
        var gelirDagilimi = await GetDagilimAsync(siteId, FaturaTipi.Gelir, false, null, null, ct);
        var duyurular = await GetDuyurularAsync(siteId, ct);
        var bankaHesaplari = await GetBankaHesaplariAsync(siteId, ct);

        return new ReportSummaryDto(
            kasalar, isTakibi, aidatTahsilat, finansalDurum,
            giderEvraklari, gelirEvraklari, odenecekFaturalar,
            giderDagilimi, gelirDagilimi, duyurular, bankaHesaplari);
    }
}
