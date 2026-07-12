namespace SiteYonetimi.Infrastructure.Entities.Shared;

/// <summary>
/// Personel başına 1:1 muhasebe entegrasyon hesap kodları. İlk 10 alan
/// GiderTanimi.Id'ye, son 7 alan HesapPlani.Id'ye (cari hesap) referans verir.
/// </summary>
public class PersonelMuhasebeEntegrasyon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid PersonelId { get; set; }

    // → GiderTanimi.Id
    public Guid? BrutUcretlerGiderTanimiId { get; set; }
    public Guid? HuzurHakkiBrutUcretlerGiderTanimiId { get; set; }
    public Guid? SgkIsverenPayiGiderTanimiId { get; set; }
    public Guid? IssizlikSigortasiIsverenPayiGiderTanimiId { get; set; }
    public Guid? PrimVeIkramiyelerGiderTanimiId { get; set; }
    public Guid? FazlaMesaiGiderTanimiId { get; set; }
    public Guid? KidemTazminatlariGiderTanimiId { get; set; }
    public Guid? IhbarTazminatlariGiderTanimiId { get; set; }
    public Guid? YolYardimiGiderTanimiId { get; set; }
    public Guid? YemekYardimiGiderTanimiId { get; set; }

    // → HesapPlani.Id (cari hesap)
    public Guid? PersonelGelirVergisiHesapId { get; set; }
    public Guid? PersonelDamgaVergisiHesapId { get; set; }
    public Guid? OdenecekSgkHesapId { get; set; }
    public Guid? AsgariGecimIndirimiHesapId { get; set; }
    public Guid? IcraKesintisiHesapId { get; set; }
    public Guid? DigerKesintilerHesapId { get; set; }
    public Guid? BesHesapId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Personel Personel { get; set; } = null!;
}
