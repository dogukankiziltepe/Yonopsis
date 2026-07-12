using SiteYonetimi.Shared.Enums;
namespace SiteYonetimi.SiteManagement.Toplantilar.DTOs;
public record ToplamtiDto(Guid Id, string Baslik, string? Aciklama, string? Gundem, DateTime ToplamtiTarihi, string? Konum, ToplamtiDurum Durum, string? Katilimcilar, string? Kararlar, bool IsActive, DateTime CreatedAt);
public record CreateToplamtiDto(string Baslik, string? Aciklama, string? Gundem, DateTime ToplamtiTarihi, string? Konum, string? Katilimcilar);
public record UpdateToplamtiDto(string Baslik, string? Aciklama, string? Gundem, DateTime ToplamtiTarihi, string? Konum, ToplamtiDurum Durum, string? Katilimcilar, string? Kararlar, bool IsActive);
