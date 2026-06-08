using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.SupportRequests.DTOs;

public record AddSupportRequestCommentDto(string Content);

public record SupportRequestCommentDto(
    Guid Id,
    Guid UserId,
    string? AuthorName,
    string Content,
    DateTime CreatedAt);

public record CreateSupportRequestDto(
    Guid UnitId,
    string Subject,
    string? Description);

public record UpdateSupportRequestDto(
    string Subject,
    string? Description);

public record UpdateSupportRequestStatusDto(SupportRequestStatus Status);

public record SupportRequestSummaryDto(
    Guid Id,
    Guid SiteId,
    Guid UserId,
    Guid UnitId,
    string? UnitDoorNumber,
    string Subject,
    SupportRequestStatus Status,
    DateTime CreatedAt,
    DateTime? ResolvedAt);

public record SupportRequestDetailDto(
    Guid Id,
    Guid SiteId,
    Guid UserId,
    Guid UnitId,
    string? UnitDoorNumber,
    string Subject,
    string? Description,
    SupportRequestStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ResolvedAt);
