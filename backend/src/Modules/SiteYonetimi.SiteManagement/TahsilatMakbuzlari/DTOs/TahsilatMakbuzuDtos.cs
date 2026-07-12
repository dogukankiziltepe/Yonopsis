using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.TahsilatMakbuzlari.DTOs;

public record TahsilatMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    string? BorcluAdi,
    Guid? KasaBankaId,
    string? KasaBankaAdi,
    Guid? BorcMakbuzuId,
    string? BorcMakbuzuEvrakNo,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateTahsilatMakbuzuDto(
    string? BorcluAdi,
    Guid? KasaBankaId,
    Guid? BorcMakbuzuId,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama);

public record UpdateTahsilatMakbuzuDto(
    string? BorcluAdi,
    Guid? KasaBankaId,
    Guid? BorcMakbuzuId,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama);
