namespace SiteYonetimi.Muhasebe.Parametreler.DTOs;

public record MuhasebeParametreDto(
    Guid Id,
    Guid? VarsayilanKasaHesapId,
    Guid? VarsayilanBankaHesapId,
    Guid? AidatGelirHesapId,
    Guid? GecikmeFaiziHesapId,
    string AlicilarAnaHesapKodu,
    string SaticilarAnaHesapKodu,
    string GiderAnaHesapKodu,
    string CariKodSablonu,
    string FisNoSablonu,
    string ParaBirimi,
    decimal? KdvOrani,
    bool OtomatikTahsilFisi,
    bool OtomatikTediyeFisi);

public class UpdateMuhasebeParametreDto
{
    public Guid? VarsayilanKasaHesapId { get; set; }
    public Guid? VarsayilanBankaHesapId { get; set; }
    public Guid? AidatGelirHesapId { get; set; }
    public Guid? GecikmeFaiziHesapId { get; set; }
    public string AlicilarAnaHesapKodu { get; set; } = "120";
    public string SaticilarAnaHesapKodu { get; set; } = "320";
    public string GiderAnaHesapKodu { get; set; } = "770";
    public string CariKodSablonu { get; set; } = "{ana}.{tur}.{sira:0000}";
    public string FisNoSablonu { get; set; } = "{yil}-{sira:0000000}";
    public string ParaBirimi { get; set; } = "TRY";
    public decimal? KdvOrani { get; set; }
    public bool OtomatikTahsilFisi { get; set; }
    public bool OtomatikTediyeFisi { get; set; }
}
