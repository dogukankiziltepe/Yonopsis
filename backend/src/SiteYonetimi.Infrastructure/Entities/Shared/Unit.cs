namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Unit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid BuildingId { get; set; }
    public bool IsDeleted { get; set; } = false;

    public Building Building { get; set; } = null!;
}
