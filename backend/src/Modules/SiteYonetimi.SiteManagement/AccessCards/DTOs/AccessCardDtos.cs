namespace SiteYonetimi.SiteManagement.AccessCards.DTOs;

public record CreateAccessCardDto(
    Guid UserId,
    Guid? UnitId,
    string CardNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes);

public record UpdateAccessCardDto(
    Guid? UnitId,
    string CardNumber,
    bool IsActive,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes);

public record AccessCardSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid UserId,
    string? PersonFullName,
    Guid? UnitId,
    string? UnitDoorNumber,
    string CardNumber,
    bool IsActive,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes,
    DateTime CreatedAt);
