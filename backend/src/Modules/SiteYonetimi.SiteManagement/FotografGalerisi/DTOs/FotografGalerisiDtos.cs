namespace SiteYonetimi.SiteManagement.FotografGalerisi.DTOs;
public record FotografGalerisiDto(Guid Id, string Baslik, string? Aciklama, string ImageUrl, int Sira, bool IsActive, DateTime CreatedAt);
public record CreateFotografGalerisiDto(string Baslik, string? Aciklama, string ImageUrl, int Sira);
public record UpdateFotografGalerisiDto(string Baslik, string? Aciklama, string ImageUrl, int Sira, bool IsActive);
