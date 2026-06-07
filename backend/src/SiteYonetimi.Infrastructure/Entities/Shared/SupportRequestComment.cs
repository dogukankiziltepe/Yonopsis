namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class SupportRequestComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid RequestId { get; set; }
    public Guid UserId { get; set; }
    public string? AuthorName { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SupportRequest Request { get; set; } = null!;
}
