namespace SiteYonetimi.SiteManagement.WebSiteAyarlari.DTOs;
public record AnaSayfaAyarDto(Guid? Id, string? SiteAdi, string? Slogan, string? KisaAciklama, string? IletisimTelefon, string? IletisimEmail, string? Adres, string? LogoUrl, string? KapakFotoUrl);
public record UpdateAnaSayfaAyarDto(string? SiteAdi, string? Slogan, string? KisaAciklama, string? IletisimTelefon, string? IletisimEmail, string? Adres, string? LogoUrl, string? KapakFotoUrl);
public record SiteTemAsiDto(Guid? Id, string? PrimaryColor, string? SecondaryColor, string? AccentColor, string? LogoUrl, string? FaviconUrl, string? FontFamily);
public record UpdateSiteTemasDto(string? PrimaryColor, string? SecondaryColor, string? AccentColor, string? LogoUrl, string? FaviconUrl, string? FontFamily);
