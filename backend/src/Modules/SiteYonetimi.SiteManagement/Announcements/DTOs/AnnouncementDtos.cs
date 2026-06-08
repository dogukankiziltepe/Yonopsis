namespace SiteYonetimi.SiteManagement.Announcements.DTOs;

public record CreateAnnouncementDto(
    string Title,
    string Content,
    bool IsPinned,
    DateTime? PublishDate,
    DateTime? ExpiryDate);

public record UpdateAnnouncementDto(
    string Title,
    string Content,
    bool IsPinned,
    DateTime? PublishDate,
    DateTime? ExpiryDate);

public record AnnouncementSummaryDto(
    Guid Id,
    Guid SiteId,
    string Title,
    bool IsPinned,
    DateTime? PublishDate,
    DateTime? ExpiryDate,
    DateTime CreatedAt);

public record AnnouncementDetailDto(
    Guid Id,
    Guid SiteId,
    Guid CreatedByUserId,
    string Title,
    string Content,
    bool IsPinned,
    DateTime? PublishDate,
    DateTime? ExpiryDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
