using SiteYonetimi.Shared.Enums;
namespace SiteYonetimi.SiteManagement.OtomatikBildirimler.DTOs;
public record OtomatikBildirimDto(Guid Id, OtomatikBildirimOlay OlayTipi, bool EpostaAktif, bool SmsAktif, bool MobilAktif, Guid? EpostaSablonuId, string? EpostaSablonuAd, Guid? SmsSablonuId, string? SmsSablonuAd, Guid? MobilSablonuId, string? MobilSablonuAd, bool IsActive, DateTime CreatedAt);
public record UpsertOtomatikBildirimDto(OtomatikBildirimOlay OlayTipi, bool EpostaAktif, bool SmsAktif, bool MobilAktif, Guid? EpostaSablonuId, Guid? SmsSablonuId, Guid? MobilSablonuId);
