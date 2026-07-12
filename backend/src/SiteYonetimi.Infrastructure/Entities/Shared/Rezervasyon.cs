namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Rezervasyon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid? TesisId { get; set; }
    public Guid? PersonId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public RezervasyonDurum Durum { get; set; } = RezervasyonDurum.Beklemede;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Nav props
    public Tesis? Tesis { get; set; }
}

public enum RezervasyonDurum
{
    Beklemede = 0,
    Onaylandi = 1,
    Reddedildi = 2,
    Iptal = 3,
}
