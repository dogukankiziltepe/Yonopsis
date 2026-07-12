namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonelTelefon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Label { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
