namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class TelefonRehberi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Unvan { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string? Dahili { get; set; }
    public string? Email { get; set; }
    public string? Departman { get; set; }
    public string? Aciklama { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
