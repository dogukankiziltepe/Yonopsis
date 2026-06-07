namespace SiteYonetimi.Tenancy.Plans.DTOs;

public record PlanDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    List<string> ModuleNames,
    DateTime CreatedAt);

public record CreatePlanDto(
    string Name,
    string? Description,
    decimal Price,
    bool IsActive);

public record UpdatePlanDto(
    string Name,
    string? Description,
    decimal Price,
    bool IsActive);

public record SetPlanModuleDto(Guid ModuleId, bool IsIncluded);
