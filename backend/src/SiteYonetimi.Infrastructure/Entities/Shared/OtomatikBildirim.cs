using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class OtomatikBildirim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public OtomatikBildirimOlay OlayTipi { get; set; }
    public bool EpostaAktif { get; set; } = false;
    public bool SmsAktif { get; set; } = false;
    public bool MobilAktif { get; set; } = false;
    public Guid? EpostaSablonuId { get; set; }
    public Guid? SmsSablonuId { get; set; }
    public Guid? MobilSablonuId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public EpostaSablonu? EpostaSablonu { get; set; }
    public SmsSablonu? SmsSablonu { get; set; }
    public MobilBildirimSablonu? MobilSablonu { get; set; }
}
