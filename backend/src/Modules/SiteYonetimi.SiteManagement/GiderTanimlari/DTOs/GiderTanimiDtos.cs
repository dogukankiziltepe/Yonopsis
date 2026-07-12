using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.GiderTanimlari.DTOs;

public record GiderTanimiDto(
    Guid Id,
    string GiderKodu,
    string Name,
    string? Description,
    Guid? GiderGrubuId,
    string? GiderGrubuName,
    DagitimSekli? DagitimSekli,
    bool BosDairelereDagit,
    int? Kdv,
    CariTuru? BorclandirilacakKisi,
    string? MuhasebeKodu,
    bool IsActive,
    int Order,
    DateTime CreatedAt
);

public record CreateGiderTanimiDto(
    string GiderKodu,
    string Name,
    string? Description,
    Guid? GiderGrubuId,
    DagitimSekli? DagitimSekli,
    bool BosDairelereDagit,
    int? Kdv,
    CariTuru? BorclandirilacakKisi,
    string? MuhasebeKodu,
    int Order = 0
);

public record UpdateGiderTanimiDto(
    string GiderKodu,
    string Name,
    string? Description,
    Guid? GiderGrubuId,
    DagitimSekli? DagitimSekli,
    bool BosDairelereDagit,
    int? Kdv,
    CariTuru? BorclandirilacakKisi,
    string? MuhasebeKodu,
    bool IsActive,
    int Order
);
