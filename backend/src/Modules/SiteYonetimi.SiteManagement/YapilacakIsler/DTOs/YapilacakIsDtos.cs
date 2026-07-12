using SiteYonetimi.Shared.Enums;
namespace SiteYonetimi.SiteManagement.YapilacakIsler.DTOs;
public record YapilacakIsDto(Guid Id, string Baslik, string? Aciklama, string? AtananKisi, YapilacakIsOncelik Oncelik, DateTime? TamamlanmaTarihi, YapilacakIsDurum Durum, bool IsActive, DateTime CreatedAt);
public record CreateYapilacakIsDto(string Baslik, string? Aciklama, string? AtananKisi, YapilacakIsOncelik Oncelik, DateTime? TamamlanmaTarihi);
public record UpdateYapilacakIsDto(string Baslik, string? Aciklama, string? AtananKisi, YapilacakIsOncelik Oncelik, DateTime? TamamlanmaTarihi, YapilacakIsDurum Durum, bool IsActive);
