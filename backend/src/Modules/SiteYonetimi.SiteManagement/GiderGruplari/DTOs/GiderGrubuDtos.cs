namespace SiteYonetimi.SiteManagement.GiderGruplari.DTOs;

public record GiderGrubuDto(Guid Id, string Name, string? Description, bool IsActive, int Order, DateTime CreatedAt);

public record CreateGiderGrubuDto(string Name, string? Description, int Order = 0);

public record UpdateGiderGrubuDto(string Name, string? Description, bool IsActive, int Order);
