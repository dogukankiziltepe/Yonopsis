using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.KayipEsya.DTOs;

public record KayipEsyaDto(
    Guid Id,
    string EsyaAdi,
    string? Aciklama,
    string? BulunanYer,
    DateTime BulunanTarih,
    string? SahipAdi,
    string? SahipIletisim,
    KayipEsyaDurum Durum,
    DateTime CreatedAt
);

public record CreateKayipEsyaDto(
    string EsyaAdi,
    string? Aciklama,
    string? BulunanYer,
    DateTime BulunanTarih,
    string? SahipAdi,
    string? SahipIletisim
);

public record UpdateKayipEsyaDto(
    string EsyaAdi,
    string? Aciklama,
    string? BulunanYer,
    DateTime BulunanTarih,
    string? SahipAdi,
    string? SahipIletisim,
    KayipEsyaDurum Durum
);
