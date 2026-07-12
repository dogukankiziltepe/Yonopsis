using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonelIzin
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }
    public DateOnly BaslangicTarihi { get; set; }
    public DateOnly BitisTarihi { get; set; }
    public PersonelIzinTuru IzinTuru { get; set; }
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
