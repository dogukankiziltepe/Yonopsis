using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.DTOs;

public record DonemDto(
    Guid Id,
    int Yil,
    string Ad,
    DateOnly BaslangicTarihi,
    DateOnly BitisTarihi,
    DonemDurumu Durum,
    int SonYevmiyeNo);

public class CreateDonemDto
{
    public int Yil { get; set; }
    public string? Ad { get; set; }
}
