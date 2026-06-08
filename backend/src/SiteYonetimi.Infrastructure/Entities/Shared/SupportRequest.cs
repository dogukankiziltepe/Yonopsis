using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class SupportRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid UserId { get; set; }
    public Guid UnitId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SupportRequestStatus Status { get; set; } = SupportRequestStatus.Open;
    public DateTime? ResolvedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit Unit { get; set; } = null!;
    public ICollection<SupportRequestComment> Comments { get; set; } = [];
}
