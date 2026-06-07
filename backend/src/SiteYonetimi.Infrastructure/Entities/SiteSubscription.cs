using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class SiteSubscription : BaseEntity
{
    public Guid SiteId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Site Site { get; set; } = null!;
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
