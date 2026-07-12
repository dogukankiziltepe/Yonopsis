using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class YapilacakIs
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string? AtananKisi { get; set; }
    public YapilacakIsOncelik Oncelik { get; set; } = YapilacakIsOncelik.Normal;
    public DateTime? TamamlanmaTarihi { get; set; }
    public YapilacakIsDurum Durum { get; set; } = YapilacakIsDurum.Beklemede;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
