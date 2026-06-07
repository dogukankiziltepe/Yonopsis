using SiteYonetimi.Shared.Entities;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities;

public class UserSite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SiteId { get; set; }
    public UserType? UserType { get; set; }
    public Guid? RoleTypeId { get; set; } // Management için zorunlu, diğerleri için opsiyonel
    public UserSiteStatus Status { get; set; } = UserSiteStatus.Pending;

    // Navigation
    public User User { get; set; } = null!;
    public Site Site { get; set; } = null!;
    public RoleType? RoleType { get; set; }
}
