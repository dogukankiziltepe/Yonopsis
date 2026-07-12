namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonelEgitim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }
    public string EgitiminKonusu { get; set; } = string.Empty;
    public string? Egitmen { get; set; }
    public string? EgitimYeri { get; set; }
    public DateOnly? BaslamaTarihi { get; set; }
    public DateOnly? BitisTarihi { get; set; }
    public decimal? ToplamSaat { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
