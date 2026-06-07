using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<SubscriptionPlanModule> PlanModules { get; set; } = new List<SubscriptionPlanModule>();
    public ICollection<SiteSubscription> SiteSubscriptions { get; set; } = new List<SiteSubscription>();
}
