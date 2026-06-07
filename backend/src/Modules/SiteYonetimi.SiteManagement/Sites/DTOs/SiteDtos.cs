using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Sites.DTOs;

public record CreateSiteDto(
    string Name,
    string? Address,
    string? District,
    string? City,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? TaxOffice,
    string? TaxNumber,
    DbMode DbMode,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string? AdminPhone);

public record UpdateSiteDto(
    string Name,
    string? Address,
    string? District,
    string? City,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? TaxOffice,
    string? TaxNumber,
    bool IsActive);

public record SiteDetailDto(
    Guid Id,
    string Name,
    int BlockCount,
    int UnitCount,
    string? Address,
    string? District,
    string? City,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? TaxOffice,
    string? TaxNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record SiteSummaryDto(
    Guid Id,
    string Name,
    string? City,
    string? District,
    bool IsActive,
    int BlockCount,
    int UnitCount);
