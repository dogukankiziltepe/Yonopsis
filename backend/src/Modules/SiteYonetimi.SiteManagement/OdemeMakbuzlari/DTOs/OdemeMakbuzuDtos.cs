namespace SiteYonetimi.SiteManagement.OdemeMakbuzlari.DTOs;

public record OdemeMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    DateTime Tarih,
    Guid CariHesapId,
    string? CariHesapAdi,
    Guid KasaBankaId,
    string? KasaBankaAdi,
    Guid GiderTanimiId,
    string? GiderTanimiAdi,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak,
    DateTime CreatedAt);

public record CreateOdemeMakbuzuDto(
    DateTime Tarih,
    Guid CariHesapId,
    Guid KasaBankaId,
    Guid GiderTanimiId,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak);

public record UpdateOdemeMakbuzuDto(
    DateTime Tarih,
    Guid CariHesapId,
    Guid KasaBankaId,
    Guid GiderTanimiId,
    decimal Tutar,
    string? Aciklama,
    bool DagitimYapilacak);
