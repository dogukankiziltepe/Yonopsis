namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class ZiyaretciGirisCikis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string GelensAdi { get; set; } = string.Empty;   // visitor name
    public string? GeldigiKisi { get; set; }                 // who they're visiting
    public Guid? UnitId { get; set; }
    public string? ZiyaretAmaci { get; set; }                // purpose
    public DateTime GirisSaati { get; set; } = DateTime.UtcNow;
    public DateTime? CikisSaati { get; set; }
    public string? Plaka { get; set; }                       // vehicle plate if any
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit? Unit { get; set; }
}
