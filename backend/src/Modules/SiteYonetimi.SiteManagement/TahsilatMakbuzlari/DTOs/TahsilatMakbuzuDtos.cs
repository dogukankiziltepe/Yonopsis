using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.TahsilatMakbuzlari.DTOs;

public record TahsilatMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    Guid? BorcluUserId,
    string? BorcluAdSoyad,
    Guid? KasaBankaId,
    string? KasaBankaAdi,
    Guid? BorcMakbuzuId,
    string? BorcMakbuzuEvrakNo,
    decimal? BorcMakbuzuKalanTutar,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateTahsilatMakbuzuDto(
    Guid? BorcluUserId,
    Guid? KasaBankaId,
    Guid? BorcMakbuzuId,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama);

public record UpdateTahsilatMakbuzuDto(
    Guid? BorcluUserId,
    Guid? KasaBankaId,
    Guid? BorcMakbuzuId,
    decimal OdemeTutari,
    OdemeTipi OdemeTipi,
    string? Aciklama);
