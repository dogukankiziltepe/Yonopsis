using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Teklif
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string? TedarikciAdi { get; set; }
    public decimal? Tutar { get; set; }
    public DateTime TeklifTarihi { get; set; }
    public DateTime? GecerlilikTarihi { get; set; }
    public TeklifDurum Durum { get; set; } = TeklifDurum.Beklemede;
    public string? Notlar { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
