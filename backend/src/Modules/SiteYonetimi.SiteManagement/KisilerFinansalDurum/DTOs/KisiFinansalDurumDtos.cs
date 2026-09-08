namespace SiteYonetimi.SiteManagement.KisilerFinansalDurum.DTOs;

public record KisiFinansalDurumSatiriDto(
    Guid PersonUserId,
    string AdSoyad,
    decimal BorcTutari,
    decimal Gecikme,
    decimal IadeEdilen,
    decimal Odenen,
    decimal Borc,
    decimal Alacak,
    decimal Bakiye,
    string BA); // "B" | "A"

public record KisiOzetDto(Guid PersonUserId, string AdSoyad, string? Email, string? Telefon);

public record FinansalHareketDto(
    Guid Id,
    string Kaynak,           // "Borc" | "Tahsilat" | "Devir"
    DateTime EvrakTarihi,
    DateTime? SonOdemeTarihi,
    string? Aciklama,
    decimal Borc,
    decimal Tazminat,
    decimal Alacak,
    decimal YurudakiBakiye);

public record GelirGrubuFinansalDto(
    Guid? GelirGrubuId,
    string GrupAdi,
    decimal Borc,
    decimal Tazminat,
    decimal Alacak,
    decimal Bakiye,
    List<FinansalHareketDto> Hareketler);

public record DaireFinansalDto(
    Guid? UnitId,
    string DoorNumber,
    decimal Borc,
    decimal Tazminat,
    decimal Alacak,
    decimal Bakiye,
    List<GelirGrubuFinansalDto> Kategoriler);

public record KisiFinansalDetayDto(
    KisiOzetDto Kisi,
    decimal ToplamBorc,
    decimal ToplamAlacak,
    decimal ToplamBakiye,
    List<DaireFinansalDto> Daireler);
