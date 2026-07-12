namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class MobilBildirimSablonu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Baslik { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty;
    public string? Kategori { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
