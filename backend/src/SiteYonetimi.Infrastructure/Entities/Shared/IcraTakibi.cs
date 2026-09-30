using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Avukat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string? BuroAdi { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Takibe Gönder ekranından başlatılan, bir borçlunun bir dairedeki vadesi geçmiş
/// borç makbuzlarını kapsayan takip kaydı. Mevcut tutar makbuzlardan canlı hesaplanır.
/// </summary>
public class IcraTakip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public DateTime TakipTarihi { get; set; }
    public Guid BorcluUserId { get; set; }          // cross-context soft ref -> MasterDb User.Id
    public string BorcluAdiSnapshot { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public decimal BaslangicTutari { get; set; }    // takip başlatıldığı andaki kalan toplamı
    public TakipDurumu Durum { get; set; } = TakipDurumu.Takipte;
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit Unit { get; set; } = null!;
    public ICollection<IcraTakipEvrak> Evraklar { get; set; } = new List<IcraTakipEvrak>();
}

public class IcraTakipEvrak
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TakipId { get; set; }
    public Guid BorcMakbuzuId { get; set; }

    public IcraTakip Takip { get; set; } = null!;
    public BorcMakbuzu BorcMakbuzu { get; set; } = null!;
}

/// <summary>Takipten "İcraya Ver" ile açılan icra dosyası. Bakiye, bağlı evraklardan canlı hesaplanır.</summary>
public class IcraDosyasi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid? TakipId { get; set; }
    public string DosyaNo { get; set; } = string.Empty;
    public DateTime IcraTarihi { get; set; }
    public IcraDurumu Durum { get; set; } = IcraDurumu.Icrada;
    public Guid BorcluUserId { get; set; }
    public string BorcluAdiSnapshot { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public Guid? AvukatId { get; set; }
    public decimal DosyaTutari { get; set; }        // icraya verildiği andaki kalan toplamı
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public IcraTakip? Takip { get; set; }
    public Unit Unit { get; set; } = null!;
    public Avukat? Avukat { get; set; }
    public ICollection<IcraDosyasiEvrak> Evraklar { get; set; } = new List<IcraDosyasiEvrak>();
}

public class IcraDosyasiEvrak
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DosyaId { get; set; }
    public Guid BorcMakbuzuId { get; set; }

    public IcraDosyasi Dosya { get; set; } = null!;
    public BorcMakbuzu BorcMakbuzu { get; set; } = null!;
}
