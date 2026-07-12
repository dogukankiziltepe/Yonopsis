using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.KasaBanka.DTOs;

public record KasaBankaDto(Guid Id, string Name, KasaBankaTipi Tip, string? BankaAdi, string? SubeAdi, string? HesapNo, string? IBAN, bool IsActive, DateTime CreatedAt);

public record CreateKasaBankaDto(string Name, KasaBankaTipi Tip, string? BankaAdi, string? SubeAdi, string? HesapNo, string? IBAN);

public record UpdateKasaBankaDto(string Name, KasaBankaTipi Tip, string? BankaAdi, string? SubeAdi, string? HesapNo, string? IBAN, bool IsActive);
