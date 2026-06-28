using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class KasaBanka
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public KasaBankaTipi Tip { get; set; } = KasaBankaTipi.Kasa;
    public string? BankaAdi { get; set; }
    public string? SubeAdi { get; set; }
    public string? HesapNo { get; set; }
    public string? IBAN { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
