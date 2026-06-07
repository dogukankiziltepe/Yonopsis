using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class SubscriptionPlanModule : BaseEntity
{
    public Guid SubscriptionPlanId { get; set; }
    public Guid ModuleId { get; set; }

    // Navigation
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
