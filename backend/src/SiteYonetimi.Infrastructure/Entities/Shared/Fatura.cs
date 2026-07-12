using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// Gelir ve Gider faturaları tek tabloda, FaturaTipi ile ayrışır.
public class Fatura
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public FaturaTipi Tip { get; set; }          // Gelir=0, Gider=1
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public DateTime FaturaTarihi { get; set; }
    public string CariAdi { get; set; } = string.Empty;
    public Guid? GelirTanimiId { get; set; }
    public Guid? GiderTanimiId { get; set; }
    public decimal ToplamTutar { get; set; }
    public string? Aciklama { get; set; }
    public DateTime? SonOdemeTarihi { get; set; }
    public FaturaOdemeDurumu OdemeDurumu { get; set; } = FaturaOdemeDurumu.Odenmedi;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public GelirTanimi? GelirTanimi { get; set; }
    public GiderTanimi? GiderTanimi { get; set; }
}
