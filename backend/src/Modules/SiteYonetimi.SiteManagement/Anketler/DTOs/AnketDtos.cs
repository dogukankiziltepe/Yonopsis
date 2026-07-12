using SiteYonetimi.Shared.Enums;
namespace SiteYonetimi.SiteManagement.Anketler.DTOs;
public record AnketDto(Guid Id, string Baslik, string? Aciklama, DateTime? BaslangicTarihi, DateTime? BitisTarihi, AnketDurum Durum, bool IsActive, DateTime CreatedAt);
public record CreateAnketDto(string Baslik, string? Aciklama, DateTime? BaslangicTarihi, DateTime? BitisTarihi);
public record UpdateAnketDto(string Baslik, string? Aciklama, DateTime? BaslangicTarihi, DateTime? BitisTarihi, AnketDurum Durum, bool IsActive);
