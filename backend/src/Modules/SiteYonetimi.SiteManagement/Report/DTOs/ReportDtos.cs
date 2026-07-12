using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Report.DTOs;

public record KasaBakiyeDto(
    Guid KasaBankaId,
    string Ad,
    KasaBankaTipi Tip,
    decimal Devir,
    decimal Giren,
    decimal Cikan,
    decimal Kalan);

public record IsTakibiOgesiDto(
    Guid Id,
    string Kaynak,           // "IsEmri" | "YapilacakIs"
    string Baslik,
    string? AtananKisi,
    string Oncelik,
    string Durum,
    DateTime? Tarih);

public record AidatTahsilatAyDto(
    string Donem,
    decimal TahsilEdilen,
    decimal TahsilEdilemeyen);

public record FinansalDurumNoktaDto(
    DateTime Tarih,
    Guid KasaBankaId,
    string KasaBankaAdi,
    decimal Bakiye);

public record EvrakDto(
    Guid Id,
    DateTime Tarih,
    string EvrakNo,
    string CariAdi,
    decimal Tutar);

public record OdenecekFaturaDto(
    Guid Id,
    string EvrakNo,
    string CariAdi,
    decimal Tutar,
    DateTime? SonOdemeTarihi);

public record DagilimDilimiDto(
    string Ad,
    decimal Tutar,
    double Yuzde);

public record DuyuruOzetDto(
    Guid Id,
    string Title,
    bool IsPinned,
    DateTime? PublishDate,
    DateTime CreatedAt);

public record BankaHesabiDto(
    Guid Id,
    string Ad,
    string? BankaAdi,
    string? SubeAdi,
    string? HesapNo,
    string? IBAN);

public record ReportSummaryDto(
    List<KasaBakiyeDto> Kasalar,
    List<IsTakibiOgesiDto> IsTakibi,
    List<AidatTahsilatAyDto> AidatTahsilat,
    List<FinansalDurumNoktaDto> FinansalDurum,
    List<EvrakDto> GiderEvraklari,
    List<EvrakDto> GelirEvraklari,
    List<OdenecekFaturaDto> OdenecekFaturalar,
    List<DagilimDilimiDto> GiderDagilimi,
    List<DagilimDilimiDto> GelirDagilimi,
    List<DuyuruOzetDto> Duyurular,
    List<BankaHesabiDto> BankaHesaplari);
