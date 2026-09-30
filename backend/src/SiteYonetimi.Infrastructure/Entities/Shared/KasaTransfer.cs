namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Kasalar/bankalar arası para transferi. Banka Hareketleri'nden bağımsız kayıttır.
/// </summary>
public class KasaTransfer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string EvrakNo { get; set; } = string.Empty;
    public string? BelgeNo { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public DateTime Tarih { get; set; }
    public Guid CikisKasaBankaId { get; set; }
    public Guid GirisKasaBankaId { get; set; }
    public decimal Tutar { get; set; }
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public KasaBanka? CikisKasaBanka { get; set; }
    public KasaBanka? GirisKasaBanka { get; set; }
}
