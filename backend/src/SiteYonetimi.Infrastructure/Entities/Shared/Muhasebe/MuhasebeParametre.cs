namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Muhasebe parametreleri — tenant (site) başına tek kayıt. Varsayılan hesaplar,
/// kodlama/numara şablonları, para birimi ve otomatik fiş ayarları.
/// </summary>
public class MuhasebeParametre
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }

    public Guid? VarsayilanKasaHesapId { get; set; }
    public Guid? VarsayilanBankaHesapId { get; set; }
    public Guid? AidatGelirHesapId { get; set; }
    public Guid? GecikmeFaiziHesapId { get; set; }

    public string AlicilarAnaHesapKodu { get; set; } = "120";
    public string SaticilarAnaHesapKodu { get; set; } = "320";
    public string GiderAnaHesapKodu { get; set; } = "770";

    public string CariKodSablonu { get; set; } = "{ana}.{tur}.{sira:0000}";
    public string FisNoSablonu { get; set; } = "{yil}-{sira:0000000}";
    public string ParaBirimi { get; set; } = "TRY";
    public decimal? KdvOrani { get; set; }

    /// <summary>Aidat/ödeme onaylandığında otomatik Tahsil fişi üretilsin mi.</summary>
    public bool OtomatikTahsilFisi { get; set; } = false;
    /// <summary>Gider kaydı için otomatik Tediye fişi üretilsin mi (ileride gider modülü gelince).</summary>
    public bool OtomatikTediyeFisi { get; set; } = false;

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
