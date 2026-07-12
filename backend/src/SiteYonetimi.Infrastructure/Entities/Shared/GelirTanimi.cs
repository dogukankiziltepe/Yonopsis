namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class GelirTanimi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? GelirGrubuId { get; set; }
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public GelirGrubu? GelirGrubu { get; set; }
}
