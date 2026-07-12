using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class Personel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }

    // ── Personel Tanımları ──────────────────────────────────────────────
    public string PersonelKodu { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Firma { get; set; }
    public string? Title { get; set; }
    public Gender? Cinsiyet { get; set; }
    public string? YemekKarti { get; set; }
    public string? Aciklama { get; set; }
    public string? Email { get; set; }
    public KanGrubu? KanGrubu { get; set; }
    public EducationStatus? OgrenimDurumu { get; set; }
    public string? OkulKurum { get; set; }
    public string? Adres { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? CikisTarihi { get; set; }
    public DateOnly? KidemTazminatiBaslamaTarihi { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? MuhasebeHesapKoduId { get; set; }

    // ── Banka Bilgileri ─────────────────────────────────────────────────
    // Cross-context ref (MasterDb BankaSubesi.Id) — no navigation property.
    public Guid? BankaSubesiId { get; set; }
    public string? BankaHesapNo { get; set; }
    public string? BankaIBAN { get; set; }

    // ── Personel İzin Yönetimi ──────────────────────────────────────────
    public int? YillikIzinHakkiGun { get; set; }

    // Eski alanlar — yeni UI'da kullanılmıyor, geriye dönük uyumluluk için korunuyor.
    public string? Department { get; set; }
    public string? Phone { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation (aynı DB)
    public HesapPlani? MuhasebeHesapKodu { get; set; }
}
