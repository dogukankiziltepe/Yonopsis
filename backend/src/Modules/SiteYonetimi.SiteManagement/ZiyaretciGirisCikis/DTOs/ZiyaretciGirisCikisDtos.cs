namespace SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.DTOs;

public record ZiyaretciGirisCikisDto(
    Guid Id,
    string GelensAdi,
    string? GeldigiKisi,
    Guid? UnitId,
    string? UnitDoorNumber,
    string? ZiyaretAmaci,
    DateTime GirisSaati,
    DateTime? CikisSaati,
    string? Plaka,
    string? Aciklama,
    DateTime CreatedAt
);

public record CreateZiyaretciGirisCikisDto(
    string GelensAdi,
    string? GeldigiKisi,
    Guid? UnitId,
    string? ZiyaretAmaci,
    DateTime GirisSaati,
    string? Plaka,
    string? Aciklama
);

public record UpdateZiyaretciGirisCikisDto(
    string GelensAdi,
    string? GeldigiKisi,
    Guid? UnitId,
    string? ZiyaretAmaci,
    DateTime GirisSaati,
    DateTime? CikisSaati,
    string? Plaka,
    string? Aciklama
);
