using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Personeller.DTOs;

public record PersonelCoreDto(
    Guid Id,
    string PersonelKodu,
    string Name,
    string? Firma,
    string Title,
    Gender? Cinsiyet,
    string? YemekKarti,
    string? Aciklama,
    string? Email,
    KanGrubu? KanGrubu,
    EducationStatus? OgrenimDurumu,
    string? OkulKurum,
    string? Adres,
    DateOnly? StartDate,
    DateOnly? CikisTarihi,
    DateOnly? KidemTazminatiBaslamaTarihi,
    bool IsActive,
    Guid? MuhasebeHesapKoduId,
    string? MuhasebeHesapKoduAdi
);

public record PersonelTelefonDto(Guid Id, string PhoneNumber, string? Label);

public record PersonelAcilDurumKisiDto(Guid Id, string AdSoyad, string? Yakinlik, string? Telefon);

public record PersonelEgitimDto(
    Guid Id, string EgitiminKonusu, string? Egitmen, string? EgitimYeri,
    DateOnly? BaslamaTarihi, DateOnly? BitisTarihi, decimal? ToplamSaat);

public record PersonelIzinDto(
    Guid Id, DateOnly BaslangicTarihi, DateOnly BitisTarihi,
    PersonelIzinTuru IzinTuru, string? Aciklama, int SureGun);

public record PersonelIzinOzetiDto(int? YillikIzinHakkiGun, int KullanilanGun, int? BakiyeGun);

public record PersonelKimlikDto(
    string? TcKimlikNo, string? Seri, string? Sira, string? BabaAdi, string? AnaAdi,
    string? OncekiSoyad, string? DogumYeri, DateOnly? DogumTarihi, MaritalStatus? MedeniHali,
    string? Il, string? Ilce, string? MahalleKoy, string? CiltNo, string? AileSiraNo,
    string? SiraNo, string? VerildigiYer, string? VerilisNedeni, string? KayitNo, DateOnly? VerilisTarihi);

public record UpdatePersonelKimlikDto(
    string? TcKimlikNo, string? Seri, string? Sira, string? BabaAdi, string? AnaAdi,
    string? OncekiSoyad, string? DogumYeri, DateOnly? DogumTarihi, MaritalStatus? MedeniHali,
    string? Il, string? Ilce, string? MahalleKoy, string? CiltNo, string? AileSiraNo,
    string? SiraNo, string? VerildigiYer, string? VerilisNedeni, string? KayitNo, DateOnly? VerilisTarihi);

public record PersonelMuhasebeEntegrasyonDto(
    Guid? BrutUcretlerGiderTanimiId, string? BrutUcretlerGiderTanimiAdi,
    Guid? HuzurHakkiBrutUcretlerGiderTanimiId, string? HuzurHakkiBrutUcretlerGiderTanimiAdi,
    Guid? SgkIsverenPayiGiderTanimiId, string? SgkIsverenPayiGiderTanimiAdi,
    Guid? IssizlikSigortasiIsverenPayiGiderTanimiId, string? IssizlikSigortasiIsverenPayiGiderTanimiAdi,
    Guid? PrimVeIkramiyelerGiderTanimiId, string? PrimVeIkramiyelerGiderTanimiAdi,
    Guid? FazlaMesaiGiderTanimiId, string? FazlaMesaiGiderTanimiAdi,
    Guid? KidemTazminatlariGiderTanimiId, string? KidemTazminatlariGiderTanimiAdi,
    Guid? IhbarTazminatlariGiderTanimiId, string? IhbarTazminatlariGiderTanimiAdi,
    Guid? YolYardimiGiderTanimiId, string? YolYardimiGiderTanimiAdi,
    Guid? YemekYardimiGiderTanimiId, string? YemekYardimiGiderTanimiAdi,
    Guid? PersonelGelirVergisiHesapId, string? PersonelGelirVergisiHesapAdi,
    Guid? PersonelDamgaVergisiHesapId, string? PersonelDamgaVergisiHesapAdi,
    Guid? OdenecekSgkHesapId, string? OdenecekSgkHesapAdi,
    Guid? AsgariGecimIndirimiHesapId, string? AsgariGecimIndirimiHesapAdi,
    Guid? IcraKesintisiHesapId, string? IcraKesintisiHesapAdi,
    Guid? DigerKesintilerHesapId, string? DigerKesintilerHesapAdi,
    Guid? BesHesapId, string? BesHesapAdi
);

public record UpdatePersonelMuhasebeEntegrasyonDto(
    Guid? BrutUcretlerGiderTanimiId,
    Guid? HuzurHakkiBrutUcretlerGiderTanimiId,
    Guid? SgkIsverenPayiGiderTanimiId,
    Guid? IssizlikSigortasiIsverenPayiGiderTanimiId,
    Guid? PrimVeIkramiyelerGiderTanimiId,
    Guid? FazlaMesaiGiderTanimiId,
    Guid? KidemTazminatlariGiderTanimiId,
    Guid? IhbarTazminatlariGiderTanimiId,
    Guid? YolYardimiGiderTanimiId,
    Guid? YemekYardimiGiderTanimiId,
    Guid? PersonelGelirVergisiHesapId,
    Guid? PersonelDamgaVergisiHesapId,
    Guid? OdenecekSgkHesapId,
    Guid? AsgariGecimIndirimiHesapId,
    Guid? IcraKesintisiHesapId,
    Guid? DigerKesintilerHesapId,
    Guid? BesHesapId
);

public record PersonelBankaBilgisiDto(Guid? BankaSubesiId, string? BankaAdi, string? SubeAdi, string? BankaHesapNo, string? BankaIBAN);

public record PersonelFullDetailDto(
    PersonelCoreDto Core,
    PersonelBankaBilgisiDto Banka,
    PersonelKimlikDto Kimlik,
    PersonelMuhasebeEntegrasyonDto MuhasebeEntegrasyon,
    List<PersonelTelefonDto> Telefonlar,
    List<PersonelAcilDurumKisiDto> AcilDurumKisileri,
    List<PersonelEgitimDto> Egitimler,
    List<PersonelIzinDto> Izinler,
    PersonelIzinOzetiDto IzinOzeti
);
