using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.IcraTakibi.DTOs;

// ── Avukat ──────────────────────────────────────────────────────────────────
public record AvukatDto(Guid Id, string AdSoyad, string? BuroAdi, string? Telefon, string? Eposta, string? Adres, bool IsActive);
public record SaveAvukatDto(string AdSoyad, string? BuroAdi, string? Telefon, string? Eposta, string? Adres, bool IsActive);
public record AvukatSecimDto(Guid Id, string AdSoyad);

// ── Takibe Gönder ───────────────────────────────────────────────────────────
/// <summary>Takibe Gönder filtreleri. GunSayisi: son ödeme tarihinden bu yana en az kaç gün geçmiş olmalı.</summary>
public record TakipAdayFiltreDto(Guid? BuildingId, Guid? UnitId, Guid? GelirTanimiId, int GunSayisi, decimal EnAzBakiye);

public record TakipAdayiDto(
    Guid BorcluUserId, string BorcluAdi, Guid UnitId, string? BlokAdi, string DoorNumber,
    int EvrakSayisi, decimal Borc, decimal Tazminat, decimal Odenen, decimal Kalan);

public record TakipBaslatSecimDto(Guid BorcluUserId, Guid UnitId);
public record TakipBaslatDto(TakipAdayFiltreDto Filtre, List<TakipBaslatSecimDto> Secimler);

// ── Takip Listesi ───────────────────────────────────────────────────────────
public record TakipListItemDto(
    Guid Id, DateTime TakipTarihi, Guid BorcluUserId, string BorcluAdi, Guid UnitId, string? BlokAdi, string DoorNumber,
    decimal BaslangicTutari, decimal MevcutTutar, TakipDurumu Durum,
    string? Telefon, string? Eposta, string? Adres, Guid? IcraDosyasiId);

public record TakipDetayDto(
    Guid Id, DateTime TakipTarihi, Guid BorcluUserId, string BorcluAdi, Guid UnitId, string? BlokAdi, string DoorNumber,
    decimal BaslangicTutari, decimal MevcutTutar, TakipDurumu Durum, string? Aciklama,
    Guid? IcraDosyasiId, string? IcraDosyaNo, List<IcraEvrakDto> Evraklar);

public record UpdateTakipDto(DateTime TakipTarihi, TakipDurumu Durum, string? Aciklama);

public record IcrayaVerDto(string DosyaNo, DateTime IcraTarihi, Guid? AvukatId, string? Aciklama);

// ── İcra Listesi ────────────────────────────────────────────────────────────
public record IcraDosyasiListItemDto(
    Guid Id, DateTime IcraTarihi, string DosyaNo, Guid BorcluUserId, string BorcluAdi, string? BlokAdi, string DoorNumber,
    string? AvukatAdi, IcraDurumu Durum, decimal DosyaTutari, decimal Bakiye);

public record IcraEvrakDto(
    Guid BorcMakbuzuId, string EvrakNo, DateTime? SonOdemeTarihi, string DoorNumber, string? Kategori,
    decimal EvrakTutari, decimal Tazminat, decimal Odenen, decimal Kalan);

public record IcraDosyasiDetayDto(
    Guid Id, string DosyaNo, DateTime IcraTarihi, IcraDurumu Durum,
    Guid BorcluUserId, string BorcluAdi, Guid UnitId, string? BlokAdi, string DoorNumber,
    Guid? AvukatId, string? AvukatAdi, string? Aciklama, decimal DosyaTutari, decimal Bakiye,
    Guid? TakipId, List<IcraEvrakDto> Evraklar);

public record UpdateIcraDosyasiDto(string DosyaNo, DateTime IcraTarihi, IcraDurumu Durum, Guid? AvukatId, string? Aciklama);

// ── İcra Raporu ─────────────────────────────────────────────────────────────
public record IcraRaporFiltreDto(
    DateTime? IlkTarih, DateTime? SonTarih, Guid? BuildingId, Guid? UnitId, Guid? BorcluUserId, Guid? AvukatId, string? DosyaNo);

public record IcraRaporSatiriDto(
    Guid Id, DateTime IcraTarihi, string DosyaNo, string BorcluAdi, string? BlokAdi, string DoorNumber,
    string? AvukatAdi, IcraDurumu Durum, int EvrakSayisi, decimal DosyaTutari, decimal Tahsilat, decimal Bakiye);
