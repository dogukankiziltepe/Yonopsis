namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class WhatsappLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    // Cross-context ref (MasterDb User.Id) — no navigation property.
    public Guid UserId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
