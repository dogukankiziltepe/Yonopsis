namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonelAcilDurumKisi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string? Yakinlik { get; set; }
    public string? Telefon { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
