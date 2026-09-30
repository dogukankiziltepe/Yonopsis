using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Kişi hesapları arası bakiye devri (örn. eski malikin borcunun yeni maliğe aktarılması).
/// Satırlar Kişilere Göre Finansal Durum bakiyesine yansır.
/// </summary>
public class HesaplarArasiVirman
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public DateTime Tarih { get; set; }
    public DateTime? BelgeTarihi { get; set; }
    public string? BelgeNo { get; set; }
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<VirmanSatiri> Satirlar { get; set; } = new List<VirmanSatiri>();
}

public class VirmanSatiri
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid VirmanId { get; set; }
    public int SiraNo { get; set; }
    public VirmanHesapTuru HesapTuru { get; set; } = VirmanHesapTuru.Kisi;
    /// <summary>
    /// HesapTuru'na göre: Kisi → MasterDb User.Id, Cari → HesapPlani.Id, Banka → KasaBanka.Id,
    /// Gider → GiderTanimi.Id, Personel → Personel.Id. Polimorfik olduğu için FK yok.
    /// </summary>
    public Guid HesapId { get; set; }
    public string? HesapAdiSnapshot { get; set; }
    public Guid? UnitId { get; set; }
    public Guid? GelirTanimiId { get; set; }       // "Kategori"
    public string? BorcDonemi { get; set; }        // "YYYY-MM"
    public string? Aciklama { get; set; }
    public decimal BorcTutari { get; set; }
    public decimal AlacakTutari { get; set; }
    public DateTime? SonOdemeTarihi { get; set; }

    // Gecikme tazminatı — bu fazda sadece kaydedilir, hesaplanmaz
    public bool GecikmeTazminatiUygula { get; set; }
    public DateTime? TazminatBaslamaTarihi { get; set; }
    public TazminatUygulamaSekli? TazminatUygulamaSekli { get; set; }
    public decimal? AylikTazminatYuzdesi { get; set; }
    public DateTime? TazminatHesapTarihi { get; set; }

    // İcra — bu fazda sadece kaydedilir
    public bool IcraTakibinde { get; set; }
    public DateTime? IcrayaVerilmeTarihi { get; set; }
    public string? IcraDosyaNo { get; set; }

    public HesaplarArasiVirman Virman { get; set; } = null!;
    public Unit? Unit { get; set; }
    public GelirTanimi? GelirTanimi { get; set; }
}
