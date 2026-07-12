using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.AnaSayaclar.DTOs;

public record AnaSayacDto(Guid Id, string Ad, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama, bool IsActive, DateTime CreatedAt);
public record CreateAnaSayacDto(string Ad, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama);
public record UpdateAnaSayacDto(string Ad, SayacTipi Tip, string? SeriNo, string? Marka, DateTime? TakimTarihi, string? Aciklama, bool IsActive);
