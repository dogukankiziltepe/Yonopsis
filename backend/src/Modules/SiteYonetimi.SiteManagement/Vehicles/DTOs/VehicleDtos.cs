namespace SiteYonetimi.SiteManagement.Vehicles.DTOs;

public record CreateVehicleDto(
    Guid UnitId,
    Guid? OwnerUserId,
    string Plate,
    string? Brand,
    string? Model,
    string? Color,
    int? Year);

public record UpdateVehicleDto(
    string Plate,
    string? Brand,
    string? Model,
    string? Color,
    int? Year,
    bool IsActive);

public record AssignVehicleToUnitDto(
    Guid UnitId,
    Guid? OwnerUserId);

public record ChangeVehicleOwnerDto(
    Guid? OwnerUserId);

public record VehicleSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid? UnitId,
    string? UnitDoorNumber,
    Guid? OwnerUserId,
    string Plate,
    string? Brand,
    string? Model,
    string? Color,
    int? Year,
    bool IsActive,
    DateTime CreatedAt);
