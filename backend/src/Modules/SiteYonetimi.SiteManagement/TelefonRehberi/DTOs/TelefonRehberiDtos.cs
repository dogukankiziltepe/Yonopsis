namespace SiteYonetimi.SiteManagement.TelefonRehberi.DTOs;
public record TelefonRehberiDto(Guid Id, string Ad, string? Unvan, string Telefon, string? Dahili, string? Email, string? Departman, string? Aciklama, bool IsActive, DateTime CreatedAt);
public record CreateTelefonRehberiDto(string Ad, string? Unvan, string Telefon, string? Dahili, string? Email, string? Departman, string? Aciklama);
public record UpdateTelefonRehberiDto(string Ad, string? Unvan, string Telefon, string? Dahili, string? Email, string? Departman, string? Aciklama, bool IsActive);
