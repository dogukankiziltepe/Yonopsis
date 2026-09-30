using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;

// ── Fiş ekranı (Hesaplar Arası Virman) ──────────────────────────────────────

public record VirmanListItemDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    DateTime Tarih,
    int SatirSayisi,
    decimal ToplamBorc,
    decimal ToplamAlacak,
    decimal Bakiye);

public record VirmanSatiriDto(
    Guid Id,
    VirmanHesapTuru HesapTuru,
    Guid HesapId,
    string? HesapAdi,
    Guid? UnitId,
    string? UnitDoorNumber,
    Guid? GelirTanimiId,
    string? GelirTanimiAdi,
    string? Aciklama,
    decimal BorcTutari,
    decimal AlacakTutari);

public record VirmanDetayDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    DateTime Tarih,
    DateTime? BelgeTarihi,
    string? BelgeNo,
    string? Aciklama,
    List<VirmanSatiriDto> Satirlar);

/// <summary>Id doluysa mevcut satır güncellenir (detay ekranındaki tazminat/icra alanları korunur), boşsa yeni satır.</summary>
public record VirmanSatiriInputDto(
    Guid? Id,
    VirmanHesapTuru HesapTuru,
    Guid HesapId,
    Guid? UnitId,
    Guid? GelirTanimiId,
    string? Aciklama,
    decimal BorcTutari,
    decimal AlacakTutari);

public record SaveVirmanDto(
    DateTime Tarih,
    DateTime? BelgeTarihi,
    string? BelgeNo,
    string? Aciklama,
    List<VirmanSatiriInputDto> Satirlar);

public record VirmanSecimDto(Guid Id, string EvrakNo, DateTime Tarih, string? BelgeNo);

// ── Satır ekranı (Virman Fişi Detayları) ────────────────────────────────────

public record VirmanSatirListItemDto(
    Guid Id,
    Guid VirmanId,
    string EvrakNo,
    DateTime Tarih,
    DateTime? BelgeTarihi,
    DateTime? SonOdemeTarihi,
    string? UnitDoorNumber,
    VirmanHesapTuru HesapTuru,
    string? HesapAdi,
    decimal BorcTutari,
    decimal AlacakTutari);

public record VirmanSatirDetayDto(
    Guid Id,
    Guid VirmanId,
    string VirmanEvrakNo,
    DateTime VirmanTarih,
    DateTime? VirmanBelgeTarihi,
    string? VirmanBelgeNo,
    string? VirmanAciklama,
    string? BorcDonemi,
    VirmanHesapTuru HesapTuru,
    Guid HesapId,
    string? HesapAdi,
    Guid? UnitId,
    Guid? GelirTanimiId,
    bool GecikmeTazminatiUygula,
    DateTime? TazminatBaslamaTarihi,
    DateTime? SonOdemeTarihi,
    TazminatUygulamaSekli? TazminatUygulamaSekli,
    decimal? AylikTazminatYuzdesi,
    DateTime? TazminatHesapTarihi,
    string? Aciklama,
    decimal BorcTutari,
    decimal AlacakTutari,
    bool IcraTakibinde,
    DateTime? IcrayaVerilmeTarihi,
    string? IcraDosyaNo);

public record SaveVirmanSatirDto(
    Guid VirmanId,
    string? BorcDonemi,
    VirmanHesapTuru HesapTuru,
    Guid HesapId,
    Guid? UnitId,
    Guid? GelirTanimiId,
    bool GecikmeTazminatiUygula,
    DateTime? TazminatBaslamaTarihi,
    DateTime? SonOdemeTarihi,
    TazminatUygulamaSekli? TazminatUygulamaSekli,
    decimal? AylikTazminatYuzdesi,
    DateTime? TazminatHesapTarihi,
    string? Aciklama,
    decimal BorcTutari,
    decimal AlacakTutari,
    bool IcraTakibinde,
    DateTime? IcrayaVerilmeTarihi,
    string? IcraDosyaNo);
