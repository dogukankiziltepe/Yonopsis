using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Raporlar.DTOs;

/// <summary>Tek bir hesap hareketi (defter/ekstre satırı, yürüyen bakiye ile).</summary>
public record HareketSatirDto(
    DateOnly FisTarihi,
    string FisNo,
    int? YevmiyeNo,
    string HesapKodu,
    string HesapAdi,
    string? Aciklama,
    decimal BorcTutar,
    decimal AlacakTutar,
    decimal YuruyenBakiye);

/// <summary>Defter-i Kebir / Muavin / Cari Ekstre ortak çıktısı (tek hesap).</summary>
public record DefterDto(
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    decimal AcilisBakiye,
    decimal ToplamBorc,
    decimal ToplamAlacak,
    decimal KapanisBakiye,
    List<HareketSatirDto> Satirlar);

/// <summary>Yevmiye defteri satırı.</summary>
public record YevmiyeSatirDto(
    int? YevmiyeNo,
    string FisNo,
    DateOnly FisTarihi,
    FisTuru FisTuru,
    string HesapKodu,
    string HesapAdi,
    string? Aciklama,
    decimal BorcTutar,
    decimal AlacakTutar);

public record YevmiyeDefteriDto(
    decimal ToplamBorc,
    decimal ToplamAlacak,
    List<YevmiyeSatirDto> Satirlar);

/// <summary>Mizan satırı (hesap bazlı borç/alacak toplamı + bakiye).</summary>
public record MizanSatirDto(
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    decimal BorcToplam,
    decimal AlacakToplam,
    decimal BorcBakiye,
    decimal AlacakBakiye);

public record MizanDto(
    decimal ToplamBorc,
    decimal ToplamAlacak,
    decimal ToplamBorcBakiye,
    decimal ToplamAlacakBakiye,
    List<MizanSatirDto> Satirlar);

/// <summary>Cari bakiye özeti (Borç/Alacak durumu ekranı).</summary>
public record CariBakiyeDto(
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    CariTuru? CariTuru,
    decimal BorcToplam,
    decimal AlacakToplam,
    decimal Bakiye);
