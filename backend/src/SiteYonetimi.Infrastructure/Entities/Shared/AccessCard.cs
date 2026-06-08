namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class AccessCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid UserId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
