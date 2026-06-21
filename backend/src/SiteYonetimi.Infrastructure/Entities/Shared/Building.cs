namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Building
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TotalFloors { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Unit> Units { get; set; } = new List<Unit>();
}
