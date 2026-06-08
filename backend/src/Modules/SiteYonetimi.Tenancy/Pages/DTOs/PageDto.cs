using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Tenancy.Pages.DTOs;

public class PageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Route { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public PermissionLevel UserPermission { get; set; }
}
