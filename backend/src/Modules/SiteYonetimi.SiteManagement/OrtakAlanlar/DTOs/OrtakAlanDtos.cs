namespace SiteYonetimi.SiteManagement.OrtakAlanlar.DTOs;

public record OrtakAlanDto(Guid Id, string Ad, string? Aciklama, string? Konum, bool IsActive, DateTime CreatedAt);
public record CreateOrtakAlanDto(string Ad, string? Aciklama, string? Konum);
public record UpdateOrtakAlanDto(string Ad, string? Aciklama, string? Konum, bool IsActive);
