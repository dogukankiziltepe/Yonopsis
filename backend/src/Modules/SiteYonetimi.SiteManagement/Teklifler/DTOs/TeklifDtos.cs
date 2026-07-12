using SiteYonetimi.Shared.Enums;
namespace SiteYonetimi.SiteManagement.Teklifler.DTOs;
public record TeklifDto(Guid Id, string Baslik, string? Aciklama, string? TedarikciAdi, decimal? Tutar, DateTime TeklifTarihi, DateTime? GecerlilikTarihi, TeklifDurum Durum, string? Notlar, bool IsActive, DateTime CreatedAt);
public record CreateTeklifDto(string Baslik, string? Aciklama, string? TedarikciAdi, decimal? Tutar, DateTime TeklifTarihi, DateTime? GecerlilikTarihi, string? Notlar);
public record UpdateTeklifDto(string Baslik, string? Aciklama, string? TedarikciAdi, decimal? Tutar, DateTime TeklifTarihi, DateTime? GecerlilikTarihi, TeklifDurum Durum, string? Notlar, bool IsActive);
