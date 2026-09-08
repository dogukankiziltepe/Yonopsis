using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// İade (refund) kaydı — bir kişiye yapılan geri ödeme. Kişilere Göre Finansal
/// Durum ekranındaki "İade Edilen" kolonu için kullanılır.
/// </summary>
public class IadeMakbuzu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.UtcNow;
    public Guid? BorcluUserId { get; set; }
    public UserType? BorcluRol { get; set; }
    public string? BorcluAdiSnapshot { get; set; }
    public Guid? KasaBankaId { get; set; }
    public decimal Tutar { get; set; }
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public KasaBanka? KasaBanka { get; set; }
}
