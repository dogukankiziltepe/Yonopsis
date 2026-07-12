using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class BorcMakbuzu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public string? Donem { get; set; }           // e.g. "2026-06"
    public DateTime? SonOdemeTarihi { get; set; }
    public Guid? UnitId { get; set; }
    public string? BorcluAdi { get; set; }
    public Guid? GelirTanimiId { get; set; }
    public decimal Tutar { get; set; }
    public decimal GecikmeTutari { get; set; } = 0;
    public decimal OdenenTutar { get; set; } = 0;
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Computed (not mapped)
    public decimal KalanTutar => Tutar + GecikmeTutari - OdenenTutar;

    // Navigation
    public Unit? Unit { get; set; }
    public GelirTanimi? GelirTanimi { get; set; }
}
