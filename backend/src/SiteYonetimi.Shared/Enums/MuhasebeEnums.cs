namespace SiteYonetimi.Shared.Enums;

/// <summary>
/// Hesabın türü: normal (tekdüzen) hesap mı, yoksa cari hesap mı.
/// </summary>
public enum HesapTipi
{
    Tekduzen = 0,
    Cari = 1
}

/// <summary>
/// Hesabın muhasebesel kategorisi (TDHP grupları).
/// </summary>
public enum HesapKategorisi
{
    Aktif = 0,
    Pasif = 1,
    Gelir = 2,
    Gider = 3,
    Maliyet = 4,
    Nazim = 5
}

/// <summary>
/// Hesabın doğal (normal) bakiye yönü.
/// </summary>
public enum NormalBakiye
{
    Borc = 0,
    Alacak = 1
}

/// <summary>
/// Cari hesabın temsil ettiği taraf.
/// </summary>
public enum CariTuru
{
    Kiraci = 0,
    EvSahibi = 1,
    Tedarikci = 2,
    Personel = 3,
    Diger = 4
}

/// <summary>
/// Muhasebe fişinin türü.
/// </summary>
public enum FisTuru
{
    Acilis = 0,
    Mahsup = 1,
    Tahsil = 2,
    Tediye = 3,
    Kapanis = 4
}

/// <summary>
/// Muhasebe fişinin durumu. Onaylandi == entegre.
/// </summary>
public enum FisDurumu
{
    Taslak = 0,
    Onaylandi = 1,
    Iptal = 2
}

/// <summary>
/// Mali dönemin durumu. Kapali döneme fiş girilemez.
/// </summary>
public enum DonemDurumu
{
    Acik = 0,
    Kapali = 1
}
