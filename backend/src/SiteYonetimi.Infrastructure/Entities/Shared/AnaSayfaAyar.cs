namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class AnaSayfaAyar
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string? SiteAdi { get; set; }
    public string? Slogan { get; set; }
    public string? KisaAciklama { get; set; }
    public string? IletisimTelefon { get; set; }
    public string? IletisimEmail { get; set; }
    public string? Adres { get; set; }
    public string? LogoUrl { get; set; }
    public string? KapakFotoUrl { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
