using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Units.DTOs;

public record UnitCoreDto(
    Guid Id,
    string DoorNumber,
    string? Code,
    Guid BuildingId,
    string? BuildingName,
    Guid? UnitTypeId,
    string? UnitTypeName,
    decimal? GrossArea,
    decimal? NetArea,
    decimal? LandShare,
    string? Floor,
    UnitStatus Status,
    decimal? MonthlyFee,
    int? ParkingCount,
    UnitDirection? Direction,
    string? Internet,
    bool HasDask);

public record UnitDetailInfoDto(
    string? Code2,
    string? Code3,
    DateTime? DeliveryDate,
    string? Description);

public record UnitRelatedPersonDto(
    Guid HistoryId,
    Guid PersonUserId,
    UserType Role,
    string FullName,
    decimal? SharePercentage,
    PetType? PetType,
    string? PetDetail,
    DateTime EntryDate,
    DateTime? ExitDate,
    string? ContactPerson,
    string? Notes,
    decimal Balance);

public record UnitVehicleDto(
    Guid Id,
    string Plate,
    string? OwnerFullName,
    string? Brand,
    string? Model,
    string? Color,
    bool IsActive);

public record UnitFullDetailDto(
    UnitCoreDto Core,
    UnitDetailInfoDto Detail,
    List<UnitRelatedPersonDto> RelatedPersons,
    List<UnitVehicleDto> Vehicles);
