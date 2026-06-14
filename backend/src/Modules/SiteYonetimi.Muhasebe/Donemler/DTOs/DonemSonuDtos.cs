using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.DTOs;

/// <summary>Bir dönemdeki hesabın net bakiyesi (borç - alacak).</summary>
public record DonemHesapBakiye(
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    HesapKategorisi Kategori,
    decimal Net);

/// <summary>Dönem sonu kapanış önizlemesi.</summary>
public record KapanisOnizlemeDto(
    Guid DonemId,
    int Yil,
    decimal ToplamGelir,
    decimal ToplamGider,
    decimal NetSonuc,
    bool Dengeli,
    List<DonemHesapBakiye> Bakiyeler);

public record DonemSonuSonucDto(
    Guid KapanisFisId,
    Guid YeniDonemId,
    Guid AcilisFisId);
