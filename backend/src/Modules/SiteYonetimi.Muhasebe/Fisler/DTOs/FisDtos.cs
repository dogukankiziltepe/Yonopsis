using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.DTOs;

/// <summary>Fiş satırı (giriş).</summary>
public class FisDetaySatirDto
{
    public Guid HesapId { get; set; }
    public decimal BorcTutar { get; set; }
    public decimal AlacakTutar { get; set; }
    public string? Aciklama { get; set; }
    public string? BelgeNo { get; set; }
}

/// <summary>Yeni fiş (başlık + satırlar).</summary>
public class CreateFisDto
{
    public FisTuru FisTuru { get; set; } = FisTuru.Mahsup;
    public DateOnly FisTarihi { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public List<FisDetaySatirDto> Detaylar { get; set; } = new();
}

/// <summary>Taslak fiş güncelleme (satırlar tamamen değiştirilir).</summary>
public class UpdateFisDto
{
    public FisTuru FisTuru { get; set; } = FisTuru.Mahsup;
    public DateOnly FisTarihi { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public List<FisDetaySatirDto> Detaylar { get; set; } = new();
}

public record FisListItemDto(
    Guid Id,
    string FisNo,
    int? YevmiyeNo,
    DateOnly FisTarihi,
    FisTuru FisTuru,
    string Aciklama,
    FisDurumu Durum,
    decimal ToplamBorc,
    decimal ToplamAlacak,
    int SatirSayisi);

public record FisDetaySatirViewDto(
    Guid Id,
    int SiraNo,
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    decimal BorcTutar,
    decimal AlacakTutar,
    string? Aciklama,
    string? BelgeNo);

public record FisDetayDto(
    Guid Id,
    Guid DonemId,
    string FisNo,
    int? YevmiyeNo,
    DateOnly FisTarihi,
    FisTuru FisTuru,
    string Aciklama,
    FisDurumu Durum,
    decimal ToplamBorc,
    decimal ToplamAlacak,
    int SatirSayisi,
    List<FisDetaySatirViewDto> Detaylar);

/// <summary>Fiş Detayları Yönetimi ekranı için düz satır.</summary>
public record FisDetaylarItemDto(
    Guid DetayId,
    Guid FisId,
    string FisNo,
    int? YevmiyeNo,
    DateOnly FisTarihi,
    FisDurumu Durum,
    int SiraNo,
    Guid HesapId,
    string HesapKodu,
    string HesapAdi,
    decimal BorcTutar,
    decimal AlacakTutar,
    string? Aciklama,
    string? BelgeNo);
