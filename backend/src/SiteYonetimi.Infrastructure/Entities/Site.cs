using SiteYonetimi.Shared.Entities;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities;

public class Site : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public DbMode DbMode { get; set; } = DbMode.Shared;
    public string? ConnectionString { get; set; } // Dedicated ise dolu, Shared ise null
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserSite> UserSites { get; set; } = new List<UserSite>();
    public ICollection<RoleType> RoleTypes { get; set; } = new List<RoleType>();
    public ICollection<SiteSubscription> SiteSubscriptions { get; set; } = new List<SiteSubscription>();
}
