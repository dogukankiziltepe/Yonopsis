using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class AnaSayac
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Ad { get; set; } = string.Empty;
    public SayacTipi Tip { get; set; } = SayacTipi.Elektrik;
    public string? SeriNo { get; set; }
    public string? Marka { get; set; }
    public DateTime? TakimTarihi { get; set; }
    public string? Aciklama { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<DaireSayac> DaireSayaclari { get; set; } = new List<DaireSayac>();
    public ICollection<SayacOkuma> Okumalar { get; set; } = new List<SayacOkuma>();
}
