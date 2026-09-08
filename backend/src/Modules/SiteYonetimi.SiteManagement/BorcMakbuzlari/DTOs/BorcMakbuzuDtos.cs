using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.BorcMakbuzlari.DTOs;

public record BorcMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime IslemTarihi,
    string? Donem,
    DateTime? SonOdemeTarihi,
    Guid? UnitId,
    string? UnitDoorNumber,
    Guid? BorcluUserId,
    string? BorcluAdSoyad,
    UserType? BorcluRol,
    string? GelirTanimiAdi,
    decimal Tutar,
    decimal GecikmeTutari,
    decimal OdenenTutar,
    decimal KalanTutar,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateBorcMakbuzuDto(
    string? Donem,
    DateTime? SonOdemeTarihi,
    Guid? UnitId,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    Guid? GelirTanimiId,
    decimal Tutar,
    string? Aciklama);

public record UpdateBorcMakbuzuDto(
    string? Donem,
    DateTime? SonOdemeTarihi,
    Guid? UnitId,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    Guid? GelirTanimiId,
    decimal Tutar,
    decimal GecikmeTutari,
    decimal OdenenTutar,
    string? Aciklama);
