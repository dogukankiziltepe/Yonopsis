namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Gider tarafı, bağımsız kasa çıkışı — belirli bir Gider Faturası'na bağlı değil.
/// </summary>
public class OdemeMakbuzu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public DateTime Tarih { get; set; }
    public Guid CariHesapId { get; set; }        // -> HesapPlani.Id (cross-navigation, aynı DbContext)
    public Guid KasaBankaId { get; set; }
    public Guid GiderTanimiId { get; set; }
    public decimal Tutar { get; set; }
    public string? Aciklama { get; set; }
    public bool DagitimYapilacak { get; set; } = false;  // bu fazda no-op
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public KasaBanka? KasaBanka { get; set; }
    public GiderTanimi? GiderTanimi { get; set; }
}
