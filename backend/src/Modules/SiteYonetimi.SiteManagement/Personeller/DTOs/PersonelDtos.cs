using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Personeller.DTOs;

public record PersonelDto(
    Guid Id,
    Guid SiteId,
    string PersonelKodu,
    string Name,
    string? Firma,
    string? Title,
    string? TcKimlikNo,
    string? Phone,
    string? Email,
    DateOnly? DogumTarihi,
    string? Aciklama,
    DateOnly? StartDate,
    DateOnly? CikisTarihi,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreatePersonelDto(
    string PersonelKodu,
    string Name,
    string? Firma,
    string Title,
    string? Email,
    DateOnly? StartDate
);

public record UpdatePersonelDto(
    string PersonelKodu,
    string Name,
    string? Firma,
    string Title,
    Gender? Cinsiyet,
    string? YemekKarti,
    string? Aciklama,
    string? Email,
    KanGrubu? KanGrubu,
    EducationStatus? OgrenimDurumu,
    string? OkulKurum,
    string? Adres,
    DateOnly? StartDate,
    DateOnly? CikisTarihi,
    DateOnly? KidemTazminatiBaslamaTarihi,
    bool IsActive,
    Guid? MuhasebeHesapKoduId,
    Guid? BankaSubesiId,
    string? BankaHesapNo,
    string? BankaIBAN,
    int? YillikIzinHakkiGun
);
