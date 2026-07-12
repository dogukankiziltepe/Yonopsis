using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Toplanti
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string? Gundem { get; set; }
    public DateTime ToplamtiTarihi { get; set; }
    public string? Konum { get; set; }
    public ToplamtiDurum Durum { get; set; } = ToplamtiDurum.Planlandilar;
    public string? Katilimcilar { get; set; }
    public string? Kararlar { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
