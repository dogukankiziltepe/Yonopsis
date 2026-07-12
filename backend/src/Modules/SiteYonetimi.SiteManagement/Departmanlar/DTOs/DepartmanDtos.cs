namespace SiteYonetimi.SiteManagement.Departmanlar.DTOs;

public record DepartmanDto(Guid Id, string Ad, string? Aciklama, bool IsActive, DateTime CreatedAt);
public record CreateDepartmanDto(string Ad, string? Aciklama);
public record UpdateDepartmanDto(string Ad, string? Aciklama, bool IsActive);
