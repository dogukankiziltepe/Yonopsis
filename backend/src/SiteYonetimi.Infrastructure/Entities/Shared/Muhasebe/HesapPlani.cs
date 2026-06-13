using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Hesap planı (Chart of Accounts). Hiyerarşik hesap ağacı; cari hesaplar da
/// bu tablonun bir parçasıdır (CariTuru dolu olan kayıtlar). Tenant (site)
/// bazında izole edilir (SiteId).
/// </summary>
public class HesapPlani
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }

    /// <summary>Örn. "120.01.0001". Tenant içinde unique.</summary>
    public string HesapKodu { get; set; } = string.Empty;
    public string HesapAdi { get; set; } = string.Empty;

    public HesapTipi HesapTipi { get; set; } = HesapTipi.Tekduzen;
    public HesapKategorisi HesapKategorisi { get; set; }
    public NormalBakiye NormalBakiye { get; set; }

    /// <summary>1=ana, 2=alt, 3=muavin/cari.</summary>
    public int Seviye { get; set; }

    /// <summary>Hiyerarşi için self-reference.</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Sadece en alt seviye (yaprak) hesaplara fiş kesilebilir.</summary>
    public bool FisKesilebilirMi { get; set; }

    /// <summary>Cari hesaplarda dolu; tekdüzen hesaplarda null.</summary>
    public CariTuru? CariTuru { get; set; }

    /// <summary>Kiracı/ev sahibi cari ise ilgili kişinin Id'si (User/UserSite).</summary>
    public Guid? PersonId { get; set; }

    /// <summary>Gider hesabı ise ilgili gider türü (varsa).</summary>
    public Guid? GiderTuruId { get; set; }

    public bool AktifMi { get; set; } = true;
    public string? Aciklama { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation (aynı DB)
    public HesapPlani? Parent { get; set; }
    public ICollection<HesapPlani> Children { get; set; } = new List<HesapPlani>();
}
