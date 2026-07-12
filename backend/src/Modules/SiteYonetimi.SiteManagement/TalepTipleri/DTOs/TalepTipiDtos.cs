namespace SiteYonetimi.SiteManagement.TalepTipleri.DTOs;

public record TalepTipiDto(Guid Id, string Ad, string? Aciklama, bool IsActive, DateTime CreatedAt);
public record CreateTalepTipiDto(string Ad, string? Aciklama);
public record UpdateTalepTipiDto(string Ad, string? Aciklama, bool IsActive);
