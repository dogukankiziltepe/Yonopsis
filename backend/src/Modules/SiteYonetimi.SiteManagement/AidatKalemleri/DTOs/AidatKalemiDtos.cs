namespace SiteYonetimi.SiteManagement.AidatKalemleri.DTOs;

public record AidatKalemiDto(
    Guid Id,
    Guid SiteId,
    string Name,
    string? Description,
    bool IsActive,
    int Order,
    DateTime CreatedAt);

public record CreateAidatKalemiDto(
    string Name,
    string? Description,
    int Order);

public record UpdateAidatKalemiDto(
    string Name,
    string? Description,
    bool IsActive,
    int Order);
