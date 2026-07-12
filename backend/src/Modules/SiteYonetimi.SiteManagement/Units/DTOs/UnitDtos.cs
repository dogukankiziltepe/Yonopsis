using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Units.DTOs;

public record CreateUnitDto(
    Guid BuildingId,
    Guid? UnitTypeId,
    string DoorNumber,
    string? Code,
    string? Floor,
    decimal? GrossArea,
    decimal? NetArea,
    decimal? LandShare,
    UnitStatus Status,
    decimal? MonthlyFee,
    int? ParkingCount,
    UnitDirection? Direction,
    string? Internet,
    bool HasDask,
    string? Description,
    string? Code2,
    string? Code3,
    DateTime? DeliveryDate);

public record UpdateUnitDto(
    Guid BuildingId,
    Guid? UnitTypeId,
    string DoorNumber,
    string? Code,
    string? Floor,
    decimal? GrossArea,
    decimal? NetArea,
    decimal? LandShare,
    UnitStatus Status,
    decimal? MonthlyFee,
    int? ParkingCount,
    UnitDirection? Direction,
    string? Internet,
    bool HasDask,
    string? Description,
    string? Code2,
    string? Code3,
    DateTime? DeliveryDate);

public record UnitSummaryDto(
    Guid Id,
    Guid BuildingId,
    string? BuildingName,
    Guid? UnitTypeId,
    string? UnitTypeName,
    string DoorNumber,
    string? Code,
    string? Floor,
    UnitStatus Status,
    decimal? MonthlyFee,
    bool HasDask,
    DateTime CreatedAt);

public record UnitDetailDto(
    Guid Id,
    Guid SiteId,
    Guid BuildingId,
    string? BuildingName,
    Guid? UnitTypeId,
    string? UnitTypeName,
    string DoorNumber,
    string? Code,
    string? Floor,
    decimal? GrossArea,
    decimal? NetArea,
    decimal? LandShare,
    UnitStatus Status,
    decimal? MonthlyFee,
    int? ParkingCount,
    UnitDirection? Direction,
    string? Internet,
    bool HasDask,
    Guid? OwnerUserId,
    string? OwnerFullName,
    Guid? TenantUserId,
    string? TenantFullName,
    string? Description,
    string? Code2,
    string? Code3,
    DateTime? DeliveryDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record AssignUserDto(Guid UserId);
