using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.DaireSayaclar.DTOs;

public record DaireSayacDto(Guid Id, Guid UnitId, string? UnitDoorNumber, Guid AnaSayacId, string? AnaSayacAdi, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama, bool IsActive, DateTime CreatedAt);
public record CreateDaireSayacDto(Guid UnitId, Guid AnaSayacId, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama);
public record UpdateDaireSayacDto(Guid UnitId, Guid AnaSayacId, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama, bool IsActive);
