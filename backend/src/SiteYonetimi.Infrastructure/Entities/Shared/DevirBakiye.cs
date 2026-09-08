using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Açılış/devir bakiyesi kaydı — dönem başında devreden borç/alacak.
/// Kişilere Göre Finansal Durum ekranında evrak listesinin en başında kırmızı
/// çubukla gösterilir.
/// </summary>
public class DevirBakiye
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.UtcNow;
    public Guid? UnitId { get; set; }
    public Guid? BorcluUserId { get; set; }
    public UserType? BorcluRol { get; set; }
    public string? BorcluAdiSnapshot { get; set; }
    public decimal Tutar { get; set; }           // pozitif = borç devri
    public string? Aciklama { get; set; } = "Devir";
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Unit? Unit { get; set; }
}
