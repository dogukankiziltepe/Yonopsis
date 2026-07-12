using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class DaireSayac
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid UnitId { get; set; }
    public Guid AnaSayacId { get; set; }
    public SayacTipi Tip { get; set; } = SayacTipi.Elektrik;
    public string? SeriNo { get; set; }
    public string? Marka { get; set; }
    public DateTime? TakimTarihi { get; set; }
    public string? Aciklama { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit? Unit { get; set; }
    public AnaSayac? AnaSayac { get; set; }
    public ICollection<SayacOkuma> Okumalar { get; set; } = new List<SayacOkuma>();
}
