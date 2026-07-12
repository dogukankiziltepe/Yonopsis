using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class BankaHareketi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid KasaBankaId { get; set; }
    public DateTime Tarih { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public string? ReferansNo { get; set; }
    public decimal Tutar { get; set; }           // positive = incoming, negative = outgoing
    public BankaHareketiDurum Durum { get; set; } = BankaHareketiDurum.Bekleyen;
    public Guid? EslestirmeId { get; set; }      // ID of matched TahsilatMakbuzu or Fatura
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public KasaBanka KasaBanka { get; set; } = null!;
}
