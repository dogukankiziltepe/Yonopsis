using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Faturalar.DTOs;

public record FaturaDto(
    Guid Id,
    FaturaTipi Tip,
    string EvrakNo,
    DateTime IslemTarihi,
    DateTime FaturaTarihi,
    string CariAdi,
    Guid? GelirTanimiId,
    string? GelirTanimiAdi,
    Guid? GiderTanimiId,
    string? GiderTanimiAdi,
    decimal ToplamTutar,
    string? Aciklama,
    DateTime? SonOdemeTarihi,
    FaturaOdemeDurumu OdemeDurumu,
    DateTime CreatedAt);

public record CreateFaturaDto(
    FaturaTipi Tip,
    DateTime FaturaTarihi,
    string CariAdi,
    Guid? GelirTanimiId,
    Guid? GiderTanimiId,
    decimal ToplamTutar,
    string? Aciklama,
    DateTime? SonOdemeTarihi);

public record UpdateFaturaDto(
    DateTime FaturaTarihi,
    string CariAdi,
    Guid? GelirTanimiId,
    Guid? GiderTanimiId,
    decimal ToplamTutar,
    string? Aciklama,
    DateTime? SonOdemeTarihi,
    FaturaOdemeDurumu? OdemeDurumu);
