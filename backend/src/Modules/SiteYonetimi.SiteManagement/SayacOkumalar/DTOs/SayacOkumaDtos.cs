namespace SiteYonetimi.SiteManagement.SayacOkumalar.DTOs;

public record SayacOkumaDto(Guid Id, Guid? AnaSayacId, string? AnaSayacAdi, Guid? DaireSayacId, string? UnitDoorNumber, DateTime OkumaTarihi, decimal OncekiEndeks, decimal SonEndeks, decimal Tuketim, string? Aciklama, DateTime CreatedAt);
public record CreateSayacOkumaDto(Guid? AnaSayacId, Guid? DaireSayacId, DateTime OkumaTarihi, decimal OncekiEndeks, decimal SonEndeks, string? Aciklama);
public record UpdateSayacOkumaDto(DateTime OkumaTarihi, decimal OncekiEndeks, decimal SonEndeks, string? Aciklama);
