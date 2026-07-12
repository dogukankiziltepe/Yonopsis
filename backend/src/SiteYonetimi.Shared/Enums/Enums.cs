namespace SiteYonetimi.Shared.Enums;

public enum UserType
{
    SuperAdmin = 1,
    Owner = 2,
    Renter = 3,
    Management = 4,
    Admin = 5
}

public enum DbMode
{
    Shared = 1,
    Dedicated = 2
}

public enum PermissionLevel
{
    Unauthorized = 0,
    ReadOnly = 1,
    ReadAndCreate = 2,
    FullAccess = 3
}

public enum UserSiteStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public enum UnitStatus
{
    Bos = 0,
    Dolu = 1,
    Kiralik = 2
}

public enum UnitDirection
{
    Kuzey = 0,
    Guney = 1,
    Dogu = 2,
    Bati = 3,
    Bilinmiyor = 4
}

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}

public enum SupportRequestStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Overdue = 2
}

public enum IsEmriOncelik
{
    Dusuk = 1,
    Normal = 2,
    Yuksek = 3,
    Kritik = 4
}

public enum IsEmriDurum
{
    YeniTalep = 0,
    Atandi = 1,
    Devam = 2,
    Tamamlandi = 3,
    Iptal = 4
}

public enum SayacTipi
{
    Elektrik = 1,
    Su = 2,
    Dogalgaz = 3,
    Diger = 4
}

public enum AracTipi
{
    Otomobil = 1,
    Kamyon = 2,
    Motosiklet = 3,
    Minibus = 4,
    Diger = 5
}

public enum OlayTipi
{
    Hirsizlik = 1,
    Vandalizm = 2,
    Kaza = 3,
    Kavga = 4,
    Yangin = 5,
    Diger = 6
}

public enum OlayDurum
{
    Acik = 0,
    Inceleniyor = 1,
    Kapandi = 2
}

public enum KayipEsyaDurum
{
    Beklemede = 0,
    Bulundu = 1,
    TeslimEdildi = 2
}

public enum OtomatikBildirimOlay
{
    AidatVadesi = 1,
    AidatOdendi = 2,
    BorcMakbuzu = 3,
    TahsilatMakbuzu = 4,
    ZiyaretciGiris = 5,
    AracGiris = 6,
    IsEmriOlusturuldu = 7,
    IsEmriTamamlandi = 8,
    Duyuru = 9,
    Diger = 10
}

public enum TeklifDurum
{
    Beklemede = 0,
    Onaylandi = 1,
    Reddedildi = 2,
    Iptal = 3
}

public enum ToplamtiDurum
{
    Planlandilar = 0,
    Tamamlandi = 1,
    Iptal = 2
}

public enum YapilacakIsDurum
{
    Beklemede = 0,
    Devam = 1,
    Tamamlandi = 2
}

public enum YapilacakIsOncelik
{
    Dusuk = 1,
    Normal = 2,
    Yuksek = 3
}

public enum AnketDurum
{
    Taslak = 0,
    Aktif = 1,
    Kapandi = 2
}

public enum FaturaTipi
{
    Gelir = 1,
    Gider = 2
}

public enum KasaBankaTipi
{
    Kasa = 1,
    BankHesabi = 2
}

public enum OdemeTipi
{
    Nakit = 1,
    KrediKarti = 2,
    HavaleEFT = 3,
    Cek = 4,
    Diger = 5
}

public enum BankaHareketiDurum
{
    Bekleyen = 0,
    Tamamlandi = 1,
    Iptal = 2
}

public enum FaturaOdemeDurumu
{
    Odenmedi = 0,
    Odendi = 1
}

