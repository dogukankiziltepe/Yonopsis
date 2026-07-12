using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.BankaHareketleri.DTOs;

public record BankaHareketiDto(
    Guid Id,
    Guid KasaBankaId,
    string KasaBankaAdi,
    DateTime Tarih,
    string Aciklama,
    string? ReferansNo,
    decimal Tutar,
    BankaHareketiDurum Durum,
    Guid? EslestirmeId,
    DateTime CreatedAt);

public record CreateBankaHareketiDto(
    Guid KasaBankaId,
    DateTime Tarih,
    string Aciklama,
    string? ReferansNo,
    decimal Tutar);

public record UpdateBankaHareketiDto(
    DateTime Tarih,
    string Aciklama,
    string? ReferansNo,
    decimal Tutar,
    BankaHareketiDurum Durum,
    Guid? EslestirmeId);
