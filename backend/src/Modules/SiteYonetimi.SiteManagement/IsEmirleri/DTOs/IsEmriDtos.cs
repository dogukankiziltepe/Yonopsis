using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.IsEmirleri.DTOs;

public record IsEmriDto(
    Guid Id,
    string Baslik,
    string? Aciklama,
    Guid? TalepTipiId,
    string? TalepTipiAdi,
    Guid? DepartmanId,
    string? DepartmanAdi,
    Guid? OrtakAlanId,
    string? OrtakAlanAdi,
    Guid? UnitId,
    string? UnitDoorNumber,
    IsEmriOncelik Oncelik,
    IsEmriDurum Durum,
    Guid? AtananKisiId,
    string? AtananKisiAdi,
    DateTime? IslemBaslangic,
    DateTime? IslemBitis,
    string? Notlar,
    DateTime CreatedAt
);

public record CreateIsEmriDto(
    string Baslik,
    string? Aciklama,
    Guid? TalepTipiId,
    Guid? DepartmanId,
    Guid? OrtakAlanId,
    Guid? UnitId,
    IsEmriOncelik Oncelik,
    string? AtananKisiAdi,
    DateTime? IslemBaslangic,
    string? Notlar
);

public record UpdateIsEmriDto(
    string Baslik,
    string? Aciklama,
    Guid? TalepTipiId,
    Guid? DepartmanId,
    Guid? OrtakAlanId,
    Guid? UnitId,
    IsEmriOncelik Oncelik,
    IsEmriDurum Durum,
    string? AtananKisiAdi,
    DateTime? IslemBaslangic,
    DateTime? IslemBitis,
    string? Notlar
);
