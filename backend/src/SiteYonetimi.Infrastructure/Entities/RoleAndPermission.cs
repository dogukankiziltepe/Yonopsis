using SiteYonetimi.Shared.Entities;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities;

public class RoleType : BaseEntity
{
    public Guid SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false; // SiteAdmin default rolü

    // Navigation
    public Site Site { get; set; } = null!;
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    public ICollection<UserSite> UserSites { get; set; } = new List<UserSite>();
}

public class Page : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Route { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public Guid? ParentPageId { get; set; }
    public int OrderIndex { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public Module Module { get; set; } = null!;
    public Page? ParentPage { get; set; }
    public ICollection<Page> SubPages { get; set; } = new List<Page>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}

public class RolePermission : BaseEntity
{
    public Guid RoleTypeId { get; set; }
    public Guid PageId { get; set; }
    public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Unauthorized;

    // Navigation
    public RoleType RoleType { get; set; } = null!;
    public Page Page { get; set; } = null!;
}
