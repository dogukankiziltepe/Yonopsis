using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Report.DTOs;

namespace SiteYonetimi.SiteManagement.Report.Services;

public interface IReportService
{
    Task<List<KasaBakiyeDto>> GetKasalarAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<IsTakibiOgesiDto>> GetIsTakibiAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<AidatTahsilatAyDto>> GetAidatTahsilatAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<FinansalDurumNoktaDto>> GetFinansalDurumAsync(Guid siteId, bool all, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<EvrakDto>> GetEvraklarAsync(Guid siteId, FaturaTipi tip, CancellationToken ct = default);
    Task<List<OdenecekFaturaDto>> GetOdenecekFaturalarAsync(Guid siteId, CancellationToken ct = default);
    Task<List<DagilimDilimiDto>> GetDagilimAsync(Guid siteId, FaturaTipi tip, bool all, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<List<DuyuruOzetDto>> GetDuyurularAsync(Guid siteId, CancellationToken ct = default);
    Task<List<BankaHesabiDto>> GetBankaHesaplariAsync(Guid siteId, CancellationToken ct = default);
    Task<ReportSummaryDto> GetSummaryAsync(Guid siteId, CancellationToken ct = default);
}
