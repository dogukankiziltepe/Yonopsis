namespace SiteYonetimi.SiteManagement.BankaHareketleri.ExcelImport.DTOs;

public record BankaHareketiImportRowDto(
    int RowIndex,
    DateTime? Tarih,
    string? Aciklama,
    string? ReferansNo,
    decimal? Tutar,
    bool IsValid,
    List<string> Hatalar);

public record BankaHareketiImportPreviewDto(
    int ToplamSatir,
    int GecerliSatir,
    List<BankaHareketiImportRowDto> Satirlar);

public record BankaHareketiImportConfirmItemDto(DateTime Tarih, string Aciklama, string? ReferansNo, decimal Tutar);

public record BankaHareketiImportConfirmDto(List<BankaHareketiImportConfirmItemDto> Items);

public record BankaHareketiImportSonucDto(int OlusturulanSayisi);
