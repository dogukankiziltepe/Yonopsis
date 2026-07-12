namespace SiteYonetimi.SiteManagement.AjandaEtkinlikleri.DTOs;
public record AjandaEtkinlikDto(Guid Id, string Baslik, string? Aciklama, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Konum, string? Renk, bool TumGun, bool IsActive, DateTime CreatedAt);
public record CreateAjandaEtkinlikDto(string Baslik, string? Aciklama, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Konum, string? Renk, bool TumGun);
public record UpdateAjandaEtkinlikDto(string Baslik, string? Aciklama, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Konum, string? Renk, bool TumGun, bool IsActive);
