namespace SiteYonetimi.SiteManagement.Buildings.DTOs;

public record CreateBuildingDto(string Name);

public record UpdateBuildingDto(string Name, bool IsActive);

public record BuildingDetailDto(
    Guid Id,
    Guid SiteId,
    string Name,
    bool IsActive,
    int UnitCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record BuildingSummaryDto(
    Guid Id,
    string Name,
    bool IsActive,
    int UnitCount);
