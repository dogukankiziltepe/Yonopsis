namespace SiteYonetimi.SiteManagement.AccessCards.DTOs;

public record CreateAccessCardDto(
    Guid UserId,
    string CardNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes);

public record UpdateAccessCardDto(
    string CardNumber,
    bool IsActive,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes);

public record AccessCardSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid UserId,
    string CardNumber,
    bool IsActive,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    string? Notes,
    DateTime CreatedAt);
