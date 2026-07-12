using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Olaylar.DTOs;

public record OlayDto(
    Guid Id,
    string Baslik,
    string Aciklama,
    DateTime OlayTarihi,
    OlayTipi Tip,
    string? Konum,
    Guid? UnitId,
    string? UnitDoorNumber,
    OlayDurum Durum,
    DateTime CreatedAt
);

public record CreateOlayDto(
    string Baslik,
    string Aciklama,
    DateTime OlayTarihi,
    OlayTipi Tip,
    string? Konum,
    Guid? UnitId
);

public record UpdateOlayDto(
    string Baslik,
    string Aciklama,
    DateTime OlayTarihi,
    OlayTipi Tip,
    string? Konum,
    Guid? UnitId,
    OlayDurum Durum
);
