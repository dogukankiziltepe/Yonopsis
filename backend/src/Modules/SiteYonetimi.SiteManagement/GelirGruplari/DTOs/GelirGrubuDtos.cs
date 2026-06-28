namespace SiteYonetimi.SiteManagement.GelirGruplari.DTOs;

public record GelirGrubuDto(Guid Id, string Name, string? Description, bool IsActive, int Order, DateTime CreatedAt);

public record CreateGelirGrubuDto(string Name, string? Description, int Order = 0);

public record UpdateGelirGrubuDto(string Name, string? Description, bool IsActive, int Order);
