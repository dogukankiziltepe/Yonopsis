using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Muhasebe fişi (yevmiye kaydı başlığı). Bir döneme bağlıdır. Onaylandığında
/// (entegre) sıralı yevmiye no atanır ve düzenlenemez hale gelir.
/// </summary>
public class MuhasebeFisi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid DonemId { get; set; }

    /// <summary>Onaylanınca atanır (dönem içinde sıralı, boşluksuz).</summary>
    public int? YevmiyeNo { get; set; }

    /// <summary>Görüntülenen fiş numarası (örn. "2026-0000123").</summary>
    public string FisNo { get; set; } = string.Empty;

    public FisTuru FisTuru { get; set; } = FisTuru.Mahsup;
    public DateOnly FisTarihi { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public FisDurumu Durum { get; set; } = FisDurumu.Taslak;

    // Denormalize toplamlar (rapor/performans)
    public decimal ToplamBorc { get; set; }
    public decimal ToplamAlacak { get; set; }
    public int SatirSayisi { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public MuhasebeDonem Donem { get; set; } = null!;
    public ICollection<MuhasebeFisiDetay> Detaylar { get; set; } = new List<MuhasebeFisiDetay>();
}

/// <summary>Muhasebe fişi satırı (borç/alacak hareketi).</summary>
public class MuhasebeFisiDetay
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FisId { get; set; }
    public Guid SiteId { get; set; }
    public int SiraNo { get; set; }

    public Guid HesapId { get; set; }
    /// <summary>Denormalize hesap kodu (rapor performansı).</summary>
    public string HesapKodu { get; set; } = string.Empty;

    public decimal BorcTutar { get; set; }
    public decimal AlacakTutar { get; set; }

    public string? Aciklama { get; set; }
    public string? BelgeNo { get; set; }

    public MuhasebeFisi Fis { get; set; } = null!;
}
