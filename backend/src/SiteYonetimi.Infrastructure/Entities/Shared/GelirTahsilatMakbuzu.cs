namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Gelir tarafı, bağımsız kasa girişi — TahsilatMakbuzu'dan farklı (o BorcMakbuzu'nu
/// kapatır); bu, herhangi bir Borç Makbuzu veya Gelir Faturasına bağlı olmayan
/// bağımsız bir gelir kaydıdır (örn. kira geliri, elektrik satış geliri vb.).
/// </summary>
public class GelirTahsilatMakbuzu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public DateTime Tarih { get; set; }
    public Guid CariHesapId { get; set; }        // -> HesapPlani.Id
    public Guid KasaBankaId { get; set; }
    public Guid GelirTanimiId { get; set; }
    public decimal Tutar { get; set; }
    public string? Aciklama { get; set; }
    public bool DagitimYapilacak { get; set; } = false;  // bu fazda no-op
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public KasaBanka? KasaBanka { get; set; }
    public GelirTanimi? GelirTanimi { get; set; }
}
