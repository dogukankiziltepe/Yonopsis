using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Unit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid BuildingId { get; set; }
    public Guid? UnitTypeId { get; set; }
    public string? Code { get; set; }
    public string DoorNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public decimal? GrossArea { get; set; }
    public decimal? NetArea { get; set; }
    public decimal? LandShare { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Bos;
    public decimal? MonthlyFee { get; set; }
    public int? ParkingCount { get; set; }
    public UnitDirection? Direction { get; set; }
    public string? Internet { get; set; }
    public bool HasDask { get; set; } = false;
    public Guid? OwnerUserId { get; set; }
    public Guid? TenantUserId { get; set; }
    public string? Description { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation (same DB)
    public Building Building { get; set; } = null!;
    public UnitType? UnitType { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
