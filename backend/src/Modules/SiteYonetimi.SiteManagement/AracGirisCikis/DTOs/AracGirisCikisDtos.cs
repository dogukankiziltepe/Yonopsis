using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.AracGirisCikis.DTOs;

public record AracGirisCikisDto(
    Guid Id,
    string Plaka,
    string? SuruculAdi,
    Guid? UnitId,
    string? UnitDoorNumber,
    AracTipi? AracTipi,
    DateTime GirisSaati,
    DateTime? CikisSaati,
    string? Aciklama,
    DateTime CreatedAt
);

public record CreateAracGirisCikisDto(
    string Plaka,
    string? SuruculAdi,
    Guid? UnitId,
    AracTipi? AracTipi,
    DateTime GirisSaati,
    string? Aciklama
);

public record UpdateAracGirisCikisDto(
    string Plaka,
    string? SuruculAdi,
    Guid? UnitId,
    AracTipi? AracTipi,
    DateTime GirisSaati,
    DateTime? CikisSaati,
    string? Aciklama
);
