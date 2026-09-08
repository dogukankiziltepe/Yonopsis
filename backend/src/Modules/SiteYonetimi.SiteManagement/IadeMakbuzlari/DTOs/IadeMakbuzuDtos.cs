using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.IadeMakbuzlari.DTOs;

public record IadeMakbuzuDto(
    Guid Id,
    string EvrakNo,
    DateTime Tarih,
    Guid? BorcluUserId,
    string? BorcluAdSoyad,
    UserType? BorcluRol,
    Guid? KasaBankaId,
    string? KasaBankaAdi,
    decimal Tutar,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateIadeMakbuzuDto(
    DateTime Tarih,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    Guid? KasaBankaId,
    decimal Tutar,
    string? Aciklama);

public record UpdateIadeMakbuzuDto(
    DateTime Tarih,
    Guid? BorcluUserId,
    UserType? BorcluRol,
    Guid? KasaBankaId,
    decimal Tutar,
    string? Aciklama);
