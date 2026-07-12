using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class BankaSubesi : BaseEntity
{
    public Guid BankaId { get; set; }
    public string SubeAdi { get; set; } = string.Empty;
    public string? SubeKodu { get; set; }
    public bool IsActive { get; set; } = true;

    public Banka Banka { get; set; } = null!;
}
