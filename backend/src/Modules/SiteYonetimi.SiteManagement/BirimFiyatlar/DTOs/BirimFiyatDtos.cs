using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.BirimFiyatlar.DTOs;

public record BirimFiyatDto(Guid Id, SayacTipi Tip, decimal Fiyat, string? Birim, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Aciklama, DateTime CreatedAt);
public record CreateBirimFiyatDto(SayacTipi Tip, decimal Fiyat, string? Birim, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Aciklama);
public record UpdateBirimFiyatDto(SayacTipi Tip, decimal Fiyat, string? Birim, DateTime BaslangicTarihi, DateTime? BitisTarihi, string? Aciklama);
