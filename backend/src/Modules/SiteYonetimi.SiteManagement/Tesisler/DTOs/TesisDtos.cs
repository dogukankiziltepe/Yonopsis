namespace SiteYonetimi.SiteManagement.Tesisler.DTOs;

public record TesisDto(Guid Id, string Name, string? Description, int? Kapasite, bool IsActive, int Order, DateTime CreatedAt);

public record CreateTesisDto(string Name, string? Description, int? Kapasite, int Order = 0);

public record UpdateTesisDto(string Name, string? Description, int? Kapasite, bool IsActive, int Order);
