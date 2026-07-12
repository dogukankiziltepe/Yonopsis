using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class TahsilatMakbuzu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public string? BorcluAdi { get; set; }
    public Guid? KasaBankaId { get; set; }
    public Guid? BorcMakbuzuId { get; set; }
    public decimal OdemeTutari { get; set; }
    public OdemeTipi OdemeTipi { get; set; } = OdemeTipi.Nakit;
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public KasaBanka? KasaBanka { get; set; }
    public BorcMakbuzu? BorcMakbuzu { get; set; }
}
