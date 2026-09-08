namespace SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.DTOs;

public record GelirTahsilatMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    DateTime Tarih,
    Guid CariHesapId,
    string? CariHesapAdi,
    Guid KasaBankaId,
    string? KasaBankaAdi,
    Guid GelirTanimiId,
    string? GelirTanimiAdi,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak,
    DateTime CreatedAt);

public record CreateGelirTahsilatMakbuzuDto(
    DateTime Tarih,
    Guid CariHesapId,
    Guid KasaBankaId,
    Guid GelirTanimiId,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak);

public record UpdateGelirTahsilatMakbuzuDto(
    DateTime Tarih,
    Guid CariHesapId,
    Guid KasaBankaId,
    Guid GelirTanimiId,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak);
