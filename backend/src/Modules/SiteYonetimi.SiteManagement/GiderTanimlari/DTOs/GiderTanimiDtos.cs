namespace SiteYonetimi.SiteManagement.GiderTanimlari.DTOs;

public record GiderTanimiDto(Guid Id, string Name, string? Description, Guid? GiderGrubuId, string? GiderGrubuName, bool IsActive, int Order, DateTime CreatedAt);

public record CreateGiderTanimiDto(string Name, string? Description, Guid? GiderGrubuId, int Order = 0);

public record UpdateGiderTanimiDto(string Name, string? Description, Guid? GiderGrubuId, bool IsActive, int Order);
