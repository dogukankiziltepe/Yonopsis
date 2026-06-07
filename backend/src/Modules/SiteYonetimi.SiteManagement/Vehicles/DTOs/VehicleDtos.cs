namespace SiteYonetimi.SiteManagement.Vehicles.DTOs;

public record CreateVehicleDto(
    Guid UserId,
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

public record VehicleSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid UserId,
    string Plate,
    string? Brand,
    string? Model,
    string? Color,
    int? Year,
    bool IsActive,
    DateTime CreatedAt);
