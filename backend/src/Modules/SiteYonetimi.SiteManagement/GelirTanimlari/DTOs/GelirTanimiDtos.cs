namespace SiteYonetimi.SiteManagement.GelirTanimlari.DTOs;

public record GelirTanimiDto(Guid Id, string Name, string? Description, Guid? GelirGrubuId, string? GelirGrubuName, bool IsActive, int Order, DateTime CreatedAt);

public record CreateGelirTanimiDto(string Name, string? Description, Guid? GelirGrubuId, int Order = 0);

public record UpdateGelirTanimiDto(string Name, string? Description, Guid? GelirGrubuId, bool IsActive, int Order);
