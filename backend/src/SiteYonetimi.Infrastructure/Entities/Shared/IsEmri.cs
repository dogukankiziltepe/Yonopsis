using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class IsEmri
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public Guid? TalepTipiId { get; set; }
    public Guid? DepartmanId { get; set; }
    public Guid? OrtakAlanId { get; set; }
    public Guid? UnitId { get; set; }
    public IsEmriOncelik Oncelik { get; set; } = IsEmriOncelik.Normal;
    public IsEmriDurum Durum { get; set; } = IsEmriDurum.YeniTalep;
    // Cross-context ref to MasterDb User — no navigation property
    public Guid? AtananKisiId { get; set; }
    public string? AtananKisiAdi { get; set; }
    public DateTime? IslemBaslangic { get; set; }
    public DateTime? IslemBitis { get; set; }
    public string? Notlar { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public TalepTipi? TalepTipi { get; set; }
    public Departman? Departman { get; set; }
    public OrtakAlan? OrtakAlan { get; set; }
    public Unit? Unit { get; set; }
}
