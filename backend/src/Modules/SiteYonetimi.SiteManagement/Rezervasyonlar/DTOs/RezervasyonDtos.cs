using SiteYonetimi.Infrastructure.Entities.Shared;

namespace SiteYonetimi.SiteManagement.Rezervasyonlar.DTOs;

public record RezervasyonDto(
    Guid Id,
    Guid SiteId,
    Guid? TesisId,
    string? TesisAdi,
    Guid? PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    RezervasyonDurum Durum,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateRezervasyonDto(
    Guid? TesisId,
    Guid? PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    RezervasyonDurum Durum,
    string? Notes
);

public record UpdateRezervasyonDto(
    Guid? TesisId,
    Guid? PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    RezervasyonDurum Durum,
    string? Notes
);
