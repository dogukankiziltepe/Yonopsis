namespace SiteYonetimi.SiteManagement.MobilBildirimSablonlari.DTOs;
public record MobilBildirimSablonuDto(Guid Id, string Ad, string Baslik, string Icerik, string? Kategori, bool IsActive, DateTime CreatedAt);
public record CreateMobilBildirimSablonuDto(string Ad, string Baslik, string Icerik, string? Kategori);
public record UpdateMobilBildirimSablonuDto(string Ad, string Baslik, string Icerik, string? Kategori, bool IsActive);
