namespace SiteYonetimi.SiteManagement.SmsSablonlari.DTOs;
public record SmsSablonuDto(Guid Id, string Ad, string Icerik, string? Kategori, bool IsActive, DateTime CreatedAt);
public record CreateSmsSablonuDto(string Ad, string Icerik, string? Kategori);
public record UpdateSmsSablonuDto(string Ad, string Icerik, string? Kategori, bool IsActive);
