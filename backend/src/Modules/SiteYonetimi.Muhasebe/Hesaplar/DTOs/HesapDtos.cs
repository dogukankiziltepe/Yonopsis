using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Hesaplar.DTOs;

/// <summary>Hiyerarşik ağaç düğümü (GetHesapPlaniTreeQuery).</summary>
public class HesapNodeDto
{
    public Guid Id { get; set; }
    public string HesapKodu { get; set; } = string.Empty;
    public string HesapAdi { get; set; } = string.Empty;
    public HesapTipi HesapTipi { get; set; }
    public HesapKategorisi HesapKategorisi { get; set; }
    public NormalBakiye NormalBakiye { get; set; }
    public int Seviye { get; set; }
    public bool FisKesilebilirMi { get; set; }
    public bool AktifMi { get; set; }
    public CariTuru? CariTuru { get; set; }
    public List<HesapNodeDto> Children { get; set; } = new();
}

/// <summary>Düz liste satırı (GetHesapListQuery / GetCariHesaplarQuery).</summary>
public record HesapListItemDto(
    Guid Id,
    string HesapKodu,
    string HesapAdi,
    HesapTipi HesapTipi,
    HesapKategorisi HesapKategorisi,
    NormalBakiye NormalBakiye,
    int Seviye,
    Guid? ParentId,
    bool FisKesilebilirMi,
    bool AktifMi,
    CariTuru? CariTuru,
    Guid? PersonId);

/// <summary>Hesap detayı (GetHesapByIdQuery).</summary>
public record HesapDetailDto(
    Guid Id,
    string HesapKodu,
    string HesapAdi,
    HesapTipi HesapTipi,
    HesapKategorisi HesapKategorisi,
    NormalBakiye NormalBakiye,
    int Seviye,
    Guid? ParentId,
    string? ParentHesapKodu,
    bool FisKesilebilirMi,
    bool AktifMi,
    CariTuru? CariTuru,
    Guid? PersonId,
    Guid? GiderTuruId,
    string? Aciklama);

/// <summary>Manuel tekdüzen hesap oluşturma.</summary>
public class CreateHesapDto
{
    public string HesapKodu { get; set; } = string.Empty;
    public string HesapAdi { get; set; } = string.Empty;
    public HesapKategorisi HesapKategorisi { get; set; }
    public NormalBakiye NormalBakiye { get; set; }
    public Guid? ParentId { get; set; }
    public bool FisKesilebilirMi { get; set; }
    public string? Aciklama { get; set; }
}

/// <summary>Hesap güncelleme (kod ve hiyerarşi değiştirilemez).</summary>
public class UpdateHesapDto
{
    public string HesapAdi { get; set; } = string.Empty;
    public HesapKategorisi HesapKategorisi { get; set; }
    public NormalBakiye NormalBakiye { get; set; }
    public bool FisKesilebilirMi { get; set; }
    public string? Aciklama { get; set; }
}

/// <summary>Manuel cari hesap oluşturma (kiracı/ev sahibi/tedarikçi/personel).</summary>
public class CreateCariHesapDto
{
    public CariTuru CariTuru { get; set; }
    public string HesapAdi { get; set; } = string.Empty;
    public Guid? PersonId { get; set; }
    public string? Aciklama { get; set; }
}
