using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class AracGirisCikis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string? SuruculAdi { get; set; }
    public Guid? UnitId { get; set; }
    public AracTipi? AracTipi { get; set; }
    public DateTime GirisSaati { get; set; } = DateTime.UtcNow;
    public DateTime? CikisSaati { get; set; }
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit? Unit { get; set; }
}
