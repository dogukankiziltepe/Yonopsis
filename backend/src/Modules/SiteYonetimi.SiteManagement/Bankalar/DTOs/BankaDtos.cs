namespace SiteYonetimi.SiteManagement.Bankalar.DTOs;

public record BankaSubesiDto(Guid Id, Guid BankaId, string SubeAdi, string? SubeKodu, bool IsActive);

public record BankaDto(Guid Id, string Name, bool IsActive, List<BankaSubesiDto> Subeler);

public record CreateBankaDto(string Name);

public record UpdateBankaDto(string Name, bool IsActive);

public record CreateBankaSubesiDto(string SubeAdi, string? SubeKodu);

public record UpdateBankaSubesiDto(string SubeAdi, string? SubeKodu, bool IsActive);
