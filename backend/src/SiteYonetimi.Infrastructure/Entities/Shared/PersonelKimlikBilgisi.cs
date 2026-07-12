using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonelKimlikBilgisi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }

    public string? TcKimlikNo { get; set; }
    public string? Seri { get; set; }
    public string? Sira { get; set; }
    public string? BabaAdi { get; set; }
    public string? AnaAdi { get; set; }
    public string? OncekiSoyad { get; set; }
    public string? DogumYeri { get; set; }
    public DateOnly? DogumTarihi { get; set; }
    public MaritalStatus? MedeniHali { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? MahalleKoy { get; set; }
    public string? CiltNo { get; set; }
    public string? AileSiraNo { get; set; }
    public string? SiraNo { get; set; }
    public string? VerildigiYer { get; set; }
    public string? VerilisNedeni { get; set; }
    public string? KayitNo { get; set; }
    public DateOnly? VerilisTarihi { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
