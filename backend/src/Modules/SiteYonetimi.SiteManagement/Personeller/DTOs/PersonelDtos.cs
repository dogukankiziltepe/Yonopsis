namespace SiteYonetimi.SiteManagement.Personeller.DTOs;

public record PersonelDto(
    Guid Id,
    Guid SiteId,
    string Name,
    string? Title,
    string? Phone,
    string? Email,
    string? Department,
    DateOnly? StartDate,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreatePersonelDto(
    string Name,
    string? Title,
    string? Phone,
    string? Email,
    string? Department,
    DateOnly? StartDate
);

public record UpdatePersonelDto(
    string Name,
    string? Title,
    string? Phone,
    string? Email,
    string? Department,
    DateOnly? StartDate,
    bool IsActive
);
