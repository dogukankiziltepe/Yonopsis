namespace SiteYonetimi.SiteManagement.EpostaSablonlari.DTOs;
public record EpostaSablonuDto(Guid Id, string Ad, string Konu, string IcerikHtml, string? IcerikText, string? Kategori, bool IsActive, DateTime CreatedAt);
public record CreateEpostaSablonuDto(string Ad, string Konu, string IcerikHtml, string? IcerikText, string? Kategori);
public record UpdateEpostaSablonuDto(string Ad, string Konu, string IcerikHtml, string? IcerikText, string? Kategori, bool IsActive);
