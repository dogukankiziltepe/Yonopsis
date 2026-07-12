using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class KayipEsya
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EsyaAdi { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string? BulunanYer { get; set; }
    public DateTime BulunanTarih { get; set; } = DateTime.UtcNow;
    public string? SahipAdi { get; set; }
    public string? SahipIletisim { get; set; }
    public KayipEsyaDurum Durum { get; set; } = KayipEsyaDurum.Beklemede;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
