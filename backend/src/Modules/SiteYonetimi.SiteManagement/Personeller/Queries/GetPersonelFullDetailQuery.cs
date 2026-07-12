using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Queries;

public record GetPersonelFullDetailQuery(Guid Id, Guid SiteId) : IRequest<Result<PersonelFullDetailDto>>;

public class GetPersonelFullDetailQueryHandler : IRequestHandler<GetPersonelFullDetailQuery, Result<PersonelFullDetailDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetPersonelFullDetailQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<PersonelFullDetailDto>> Handle(GetPersonelFullDetailQuery request, CancellationToken cancellationToken)
    {
        var personel = await _db.Personeller
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (personel is null) return Result<PersonelFullDetailDto>.Failure("Personel bulunamadı.");

        string? muhasebeHesapKoduAdi = null;
        if (personel.MuhasebeHesapKoduId is not null)
        {
            muhasebeHesapKoduAdi = await _db.HesapPlani
                .Where(h => h.Id == personel.MuhasebeHesapKoduId)
                .Select(h => h.HesapKodu + " - " + h.HesapAdi)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var core = new PersonelCoreDto(
            personel.Id, personel.PersonelKodu, personel.Name, personel.Firma, personel.Title,
            personel.Cinsiyet, personel.YemekKarti, personel.Aciklama, personel.Email,
            personel.KanGrubu, personel.OgrenimDurumu, personel.OkulKurum, personel.Adres,
            personel.StartDate, personel.CikisTarihi, personel.KidemTazminatiBaslamaTarihi,
            personel.IsActive, personel.MuhasebeHesapKoduId, muhasebeHesapKoduAdi);

        string? bankaAdi = null, subeAdi = null;
        if (personel.BankaSubesiId is not null)
        {
            var sube = await _masterDb.BankaSubeleri
                .Include(x => x.Banka)
                .FirstOrDefaultAsync(x => x.Id == personel.BankaSubesiId, cancellationToken);
            bankaAdi = sube?.Banka.Name;
            subeAdi = sube?.SubeAdi;
        }
        var banka = new PersonelBankaBilgisiDto(personel.BankaSubesiId, bankaAdi, subeAdi, personel.BankaHesapNo, personel.BankaIBAN);

        var kimlikEntity = await _db.PersonelKimlikBilgileri
            .FirstOrDefaultAsync(x => x.PersonelId == request.Id, cancellationToken);
        var kimlik = new PersonelKimlikDto(
            kimlikEntity?.TcKimlikNo, kimlikEntity?.Seri, kimlikEntity?.Sira, kimlikEntity?.BabaAdi,
            kimlikEntity?.AnaAdi, kimlikEntity?.OncekiSoyad, kimlikEntity?.DogumYeri, kimlikEntity?.DogumTarihi,
            kimlikEntity?.MedeniHali, kimlikEntity?.Il, kimlikEntity?.Ilce, kimlikEntity?.MahalleKoy,
            kimlikEntity?.CiltNo, kimlikEntity?.AileSiraNo, kimlikEntity?.SiraNo, kimlikEntity?.VerildigiYer,
            kimlikEntity?.VerilisNedeni, kimlikEntity?.KayitNo, kimlikEntity?.VerilisTarihi);

        var muhasebeEntity = await _db.PersonelMuhasebeEntegrasyonlari
            .FirstOrDefaultAsync(x => x.PersonelId == request.Id, cancellationToken);

        var giderTanimiIds = new[]
        {
            muhasebeEntity?.BrutUcretlerGiderTanimiId, muhasebeEntity?.HuzurHakkiBrutUcretlerGiderTanimiId,
            muhasebeEntity?.SgkIsverenPayiGiderTanimiId, muhasebeEntity?.IssizlikSigortasiIsverenPayiGiderTanimiId,
            muhasebeEntity?.PrimVeIkramiyelerGiderTanimiId, muhasebeEntity?.FazlaMesaiGiderTanimiId,
            muhasebeEntity?.KidemTazminatlariGiderTanimiId, muhasebeEntity?.IhbarTazminatlariGiderTanimiId,
            muhasebeEntity?.YolYardimiGiderTanimiId, muhasebeEntity?.YemekYardimiGiderTanimiId
        }.Where(x => x is not null).Select(x => x!.Value).Distinct().ToList();

        var hesapIds = new[]
        {
            muhasebeEntity?.PersonelGelirVergisiHesapId, muhasebeEntity?.PersonelDamgaVergisiHesapId,
            muhasebeEntity?.OdenecekSgkHesapId, muhasebeEntity?.AsgariGecimIndirimiHesapId,
            muhasebeEntity?.IcraKesintisiHesapId, muhasebeEntity?.DigerKesintilerHesapId,
            muhasebeEntity?.BesHesapId, personel.MuhasebeHesapKoduId
        }.Where(x => x is not null).Select(x => x!.Value).Distinct().ToList();

        var giderTanimiAdlari = giderTanimiIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _db.GiderTanimlari.Where(x => giderTanimiIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.GiderKodu + " - " + x.Name, cancellationToken);

        var hesapAdlari = hesapIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _db.HesapPlani.Where(x => hesapIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.HesapKodu + " - " + x.HesapAdi, cancellationToken);

        string? GT(Guid? id) => id is not null && giderTanimiAdlari.TryGetValue(id.Value, out var n) ? n : null;
        string? H(Guid? id) => id is not null && hesapAdlari.TryGetValue(id.Value, out var n) ? n : null;

        var muhasebe = new PersonelMuhasebeEntegrasyonDto(
            muhasebeEntity?.BrutUcretlerGiderTanimiId, GT(muhasebeEntity?.BrutUcretlerGiderTanimiId),
            muhasebeEntity?.HuzurHakkiBrutUcretlerGiderTanimiId, GT(muhasebeEntity?.HuzurHakkiBrutUcretlerGiderTanimiId),
            muhasebeEntity?.SgkIsverenPayiGiderTanimiId, GT(muhasebeEntity?.SgkIsverenPayiGiderTanimiId),
            muhasebeEntity?.IssizlikSigortasiIsverenPayiGiderTanimiId, GT(muhasebeEntity?.IssizlikSigortasiIsverenPayiGiderTanimiId),
            muhasebeEntity?.PrimVeIkramiyelerGiderTanimiId, GT(muhasebeEntity?.PrimVeIkramiyelerGiderTanimiId),
            muhasebeEntity?.FazlaMesaiGiderTanimiId, GT(muhasebeEntity?.FazlaMesaiGiderTanimiId),
            muhasebeEntity?.KidemTazminatlariGiderTanimiId, GT(muhasebeEntity?.KidemTazminatlariGiderTanimiId),
            muhasebeEntity?.IhbarTazminatlariGiderTanimiId, GT(muhasebeEntity?.IhbarTazminatlariGiderTanimiId),
            muhasebeEntity?.YolYardimiGiderTanimiId, GT(muhasebeEntity?.YolYardimiGiderTanimiId),
            muhasebeEntity?.YemekYardimiGiderTanimiId, GT(muhasebeEntity?.YemekYardimiGiderTanimiId),
            muhasebeEntity?.PersonelGelirVergisiHesapId, H(muhasebeEntity?.PersonelGelirVergisiHesapId),
            muhasebeEntity?.PersonelDamgaVergisiHesapId, H(muhasebeEntity?.PersonelDamgaVergisiHesapId),
            muhasebeEntity?.OdenecekSgkHesapId, H(muhasebeEntity?.OdenecekSgkHesapId),
            muhasebeEntity?.AsgariGecimIndirimiHesapId, H(muhasebeEntity?.AsgariGecimIndirimiHesapId),
            muhasebeEntity?.IcraKesintisiHesapId, H(muhasebeEntity?.IcraKesintisiHesapId),
            muhasebeEntity?.DigerKesintilerHesapId, H(muhasebeEntity?.DigerKesintilerHesapId),
            muhasebeEntity?.BesHesapId, H(muhasebeEntity?.BesHesapId));

        var telefonlar = await _db.PersonelTelefonlari
            .Where(x => x.PersonelId == request.Id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new PersonelTelefonDto(x.Id, x.PhoneNumber, x.Label))
            .ToListAsync(cancellationToken);

        var acilDurumKisileri = await _db.PersonelAcilDurumKisileri
            .Where(x => x.PersonelId == request.Id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new PersonelAcilDurumKisiDto(x.Id, x.AdSoyad, x.Yakinlik, x.Telefon))
            .ToListAsync(cancellationToken);

        var egitimler = await _db.PersonelEgitimleri
            .Where(x => x.PersonelId == request.Id)
            .OrderByDescending(x => x.BaslamaTarihi)
            .Select(x => new PersonelEgitimDto(x.Id, x.EgitiminKonusu, x.Egitmen, x.EgitimYeri, x.BaslamaTarihi, x.BitisTarihi, x.ToplamSaat))
            .ToListAsync(cancellationToken);

        var izinEntities = await _db.PersonelIzinleri
            .Where(x => x.PersonelId == request.Id)
            .OrderByDescending(x => x.BaslangicTarihi)
            .ToListAsync(cancellationToken);
        var izinler = izinEntities
            .Select(x => new PersonelIzinDto(x.Id, x.BaslangicTarihi, x.BitisTarihi, x.IzinTuru,
                x.Aciklama, x.BitisTarihi.DayNumber - x.BaslangicTarihi.DayNumber + 1))
            .ToList();

        var kullanilanGun = izinEntities
            .Where(x => x.IzinTuru == PersonelIzinTuru.YillikIzin)
            .Sum(x => x.BitisTarihi.DayNumber - x.BaslangicTarihi.DayNumber + 1);
        var izinOzeti = new PersonelIzinOzetiDto(
            personel.YillikIzinHakkiGun, kullanilanGun,
            personel.YillikIzinHakkiGun is not null ? personel.YillikIzinHakkiGun.Value - kullanilanGun : null);

        var dto = new PersonelFullDetailDto(core, banka, kimlik, muhasebe, telefonlar, acilDurumKisileri, egitimler, izinler, izinOzeti);
        return Result<PersonelFullDetailDto>.Success(dto);
    }
}
