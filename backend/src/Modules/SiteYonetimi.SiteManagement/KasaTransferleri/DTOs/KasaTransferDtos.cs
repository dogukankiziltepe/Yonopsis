namespace SiteYonetimi.SiteManagement.KasaTransferleri.DTOs;

public record KasaTransferDto(
    Guid Id,
    string EvrakNo,
    string? BelgeNo,
    DateTime IslemTarihi,
    DateTime Tarih,
    Guid CikisKasaBankaId,
    string? CikisKasaBankaAdi,
    Guid GirisKasaBankaId,
    string? GirisKasaBankaAdi,
    decimal Tutar,
    string? Aciklama,
    DateTime CreatedAt);

public record CreateKasaTransferDto(
    DateTime Tarih,
    string? BelgeNo,
    Guid CikisKasaBankaId,
    Guid GirisKasaBankaId,
    decimal Tutar,
    string? Aciklama);

public record UpdateKasaTransferDto(
    DateTime Tarih,
    string? BelgeNo,
    Guid CikisKasaBankaId,
    Guid GirisKasaBankaId,
    decimal Tutar,
    string? Aciklama);
