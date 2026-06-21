using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Mali dönem (yıl bazlı). Fişler bir döneme bağlıdır; kapalı döneme fiş
/// girilemez. Yevmiye numarası bu kayıt üzerindeki sayaçtan üretilir.
/// </summary>
public class MuhasebeDonem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }

    /// <summary>Mali yıl (örn. 2026).</summary>
    public int Yil { get; set; }

    /// <summary>Örn. "2026 Mali Yılı".</summary>
    public string Ad { get; set; } = string.Empty;

    public DateOnly BaslangicTarihi { get; set; }
    public DateOnly BitisTarihi { get; set; }

    public DonemDurumu Durum { get; set; } = DonemDurumu.Acik;

    /// <summary>Dönem içi yevmiye sayacı. Fiş onaylandığında atomik artırılır.</summary>
    public int SonYevmiyeNo { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
