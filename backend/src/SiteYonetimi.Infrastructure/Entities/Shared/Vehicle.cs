namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Vehicle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    // Phase 1: nullable. Phase 2 migration makes this NOT NULL after data backfill.
    public Guid? UnitId { get; set; }
    // Renamed from UserId. Cross-context ref (MasterDb) — no navigation property.
    public Guid? OwnerUserId { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public int? Year { get; set; }
    public string? HgsNo { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Unit? Unit { get; set; }
}
