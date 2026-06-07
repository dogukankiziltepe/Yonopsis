namespace SiteYonetimi.SiteManagement.UnitTypes.DTOs;

public record CreateUnitTypeDto(string Name);

public record UpdateUnitTypeDto(string Name, bool IsActive);

public record UnitTypeDto(
    Guid Id,
    Guid SiteId,
    string Name,
    bool IsActive,
    DateTime CreatedAt);
