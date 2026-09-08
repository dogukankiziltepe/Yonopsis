using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.DevirBakiyeleri.DTOs;

public record DevirBakiyeDto(
    Guid Id,
    string EvrakNo,
    DateTime Tarih,
    Guid? UnitId,
    string? UnitDoorNumber,
    Guid? BorcluUserId,
    string? BorcluAdSoyad,
    UserType? BorcluRol,
    decimal Tutar,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateDevirBakiyeDto(
    DateTime Tarih,
    Guid? UnitId,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    decimal Tutar,
    string? Aciklama);

public record UpdateDevirBakiyeDto(
    DateTime Tarih,
    Guid? UnitId,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    decimal Tutar,
    string? Aciklama);
