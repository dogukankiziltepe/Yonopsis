using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Olay
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime OlayTarihi { get; set; } = DateTime.UtcNow;
    public OlayTipi Tip { get; set; } = OlayTipi.Diger;
    public string? Konum { get; set; }
    public Guid? UnitId { get; set; }
    public OlayDurum Durum { get; set; } = OlayDurum.Acik;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit? Unit { get; set; }
}
