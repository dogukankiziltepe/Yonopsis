using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.BorcMakbuzlari.TopluBorclandirma.DTOs;

public record GelirTanimiSecimDto(Guid Id, string Name);

public record TopluBorclandirmaTemplateRequestDto(
    List<Guid> GelirTanimiIds,
    UserType BorcluRolTercihi, // Owner veya Renter
    string? Donem,
    DateTime? SonOdemeTarihi);

public record TopluBorclandirmaPreviewItemDto(
    Guid UnitId,
    string UnitDoorNumber,
    string? BuildingName,
    Guid GelirTanimiId,
    string GelirTanimiAdi,
    decimal Tutar,
    string? Aciklama,
    Guid? BorcluUserId,
    string? BorcluAdSoyad,
    UserType? BorcluRol,
    bool Mukerrer,
    List<string> Uyarilar);

public record TopluBorclandirmaPreviewDto(
    int ToplamKombinasyon,
    decimal ToplamTutar,
    int MukerrerSayisi,
    string? Donem,
    DateTime? SonOdemeTarihi,
    List<TopluBorclandirmaPreviewItemDto> Items,
    List<string> SatirHatalari);

public record TopluBorclandirmaConfirmItemDto(
    Guid UnitId,
    Guid GelirTanimiId,
    decimal Tutar,
    string? Aciklama,
    Guid? BorcluUserId,
    UserType? BorcluRol);

public record TopluBorclandirmaConfirmDto(
    string? Donem,
    DateTime? SonOdemeTarihi,
    List<TopluBorclandirmaConfirmItemDto> Items);

public record TopluBorclandirmaSonucDto(
    Guid BatchId,
    int OlusturulanSayisi);
