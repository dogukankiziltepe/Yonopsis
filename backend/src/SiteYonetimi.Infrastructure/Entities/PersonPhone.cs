using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class PersonPhone : BaseEntity
{
    public Guid UserSiteId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Label { get; set; }

    // Navigation
    public UserSite UserSite { get; set; } = null!;
}
