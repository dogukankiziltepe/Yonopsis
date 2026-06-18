using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Muhasebe.Donemler.Services;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.Commands;

/// <summary>
/// Dönem sonu kapanış: tüm hesapları sıfırlayan kapanış fişi üretir, dönemi
/// kilitler, bir sonraki dönemi açar ve bilanço bakiyelerini taşıyan açılış
/// fişi üretir. Dönem net sonucu sonuç hesabına (570) yazılır.
/// </summary>
public record DonemSonuKapanisCommand(Guid SiteId, Guid DonemId) : IRequest<Result<DonemSonuSonucDto>>;

public class DonemSonuKapanisCommandHandler : IRequestHandler<DonemSonuKapanisCommand, Result<DonemSonuSonucDto>>
{
    private const string SonucHesapKodu = "570";

    private readonly SharedTenantDbContext _db;
    private readonly IDonemSonuService _donemSonuService;
    private readonly IFisService _fisService;

    public DonemSonuKapanisCommandHandler(
        SharedTenantDbContext db, IDonemSonuService donemSonuService, IFisService fisService)
    {
        _db = db;
        _donemSonuService = donemSonuService;
        _fisService = fisService;
    }

    public async Task<Result<DonemSonuSonucDto>> Handle(DonemSonuKapanisCommand request, CancellationToken cancellationToken)
    {
        var donem = await _db.MuhasebeDonemler
            .FirstOrDefaultAsync(d => d.SiteId == request.SiteId && d.Id == request.DonemId, cancellationToken);
        if (donem is null)
            return Result<DonemSonuSonucDto>.Failure("Dönem bulunamadı.");
        if (donem.Durum == DonemDurumu.Kapali)
            return Result<DonemSonuSonucDto>.Failure("Dönem zaten kapalı.");

        var bakiyeler = await _donemSonuService.HesapBakiyeleriAsync(request.SiteId, request.DonemId, cancellationToken);
        if (bakiyeler.Sum(b => b.Net) != 0)
            return Result<DonemSonuSonucDto>.Failure("Defter dengesiz; kapanış yapılamaz.");

        await using var tx = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            // 1) Kapanış fişi — tüm bakiyeleri sıfırla.
            var kapanisId = Guid.NewGuid();
            var kapanisDetaylar = new List<MuhasebeFisiDetay>();
            var sira = 1;
            foreach (var b in bakiyeler)
            {
                kapanisDetaylar.Add(YeniDetay(kapanisId, request.SiteId, sira++, b.HesapId, b.HesapKodu,
                    borc: b.Net < 0 ? -b.Net : 0, alacak: b.Net > 0 ? b.Net : 0));
            }

            donem.SonYevmiyeNo += 1;
            var kapanisFis = YeniFis(kapanisId, request.SiteId, donem.Id, donem.SonYevmiyeNo,
                await _fisService.GenerateFisNoAsync(request.SiteId, donem.Yil, cancellationToken),
                FisTuru.Kapanis, donem.BitisTarihi, $"{donem.Yil} dönem sonu kapanış fişi", kapanisDetaylar);
            _db.MuhasebeFisleri.Add(kapanisFis);

            donem.Durum = DonemDurumu.Kapali;
            donem.UpdatedAt = DateTime.UtcNow;

            // 2) Yeni dönem (yoksa oluştur).
            var yeniYil = donem.Yil + 1;
            var yeniDonem = await _db.MuhasebeDonemler
                .FirstOrDefaultAsync(d => d.SiteId == request.SiteId && d.Yil == yeniYil, cancellationToken);
            if (yeniDonem is null)
            {
                yeniDonem = new MuhasebeDonem
                {
                    SiteId = request.SiteId,
                    Yil = yeniYil,
                    Ad = $"{yeniYil} Mali Yılı",
                    BaslangicTarihi = new DateOnly(yeniYil, 1, 1),
                    BitisTarihi = new DateOnly(yeniYil, 12, 31),
                    Durum = DonemDurumu.Acik,
                    SonYevmiyeNo = 0
                };
                _db.MuhasebeDonemler.Add(yeniDonem);
            }

            // 3) Açılış fişi — yalnızca bilanço (Aktif/Pasif) hesapları taşı; net sonucu 570'e yaz.
            var bilanco = bakiyeler
                .Where(b => b.Kategori is HesapKategorisi.Aktif or HesapKategorisi.Pasif)
                .ToList();

            var acilisId = Guid.NewGuid();
            var acilisDetaylar = new List<MuhasebeFisiDetay>();
            var asira = 1;
            foreach (var b in bilanco)
            {
                acilisDetaylar.Add(YeniDetay(acilisId, request.SiteId, asira++, b.HesapId, b.HesapKodu,
                    borc: b.Net > 0 ? b.Net : 0, alacak: b.Net < 0 ? -b.Net : 0));
            }

            var bilancoNet = bilanco.Sum(b => b.Net);
            Guid acilisFisId = Guid.Empty;
            if (acilisDetaylar.Count > 0)
            {
                if (bilancoNet != 0)
                {
                    var sonucHesap = await EnsureSonucHesapAsync(request.SiteId, cancellationToken);
                    acilisDetaylar.Add(YeniDetay(acilisId, request.SiteId, asira++, sonucHesap.Id, sonucHesap.HesapKodu,
                        borc: bilancoNet < 0 ? -bilancoNet : 0, alacak: bilancoNet > 0 ? bilancoNet : 0));
                }

                yeniDonem.SonYevmiyeNo += 1;
                var acilisFis = YeniFis(acilisId, request.SiteId, yeniDonem.Id, yeniDonem.SonYevmiyeNo,
                    await _fisService.GenerateFisNoAsync(request.SiteId, yeniDonem.Yil, cancellationToken),
                    FisTuru.Acilis, yeniDonem.BaslangicTarihi, $"{yeniDonem.Yil} dönem açılış fişi", acilisDetaylar);
                _db.MuhasebeFisleri.Add(acilisFis);
                acilisFisId = acilisFis.Id;
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result<DonemSonuSonucDto>.Success(new DonemSonuSonucDto(kapanisId, yeniDonem.Id, acilisFisId));
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<HesapPlani> EnsureSonucHesapAsync(Guid siteId, CancellationToken ct)
    {
        var hesap = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == SonucHesapKodu, ct);
        if (hesap is not null) return hesap;

        hesap = new HesapPlani
        {
            SiteId = siteId,
            HesapKodu = SonucHesapKodu,
            HesapAdi = "DÖNEM NET KÂRI / GEÇMİŞ YILLAR SONUÇLARI",
            HesapTipi = HesapTipi.Tekduzen,
            HesapKategorisi = HesapKategorisi.Pasif,
            NormalBakiye = NormalBakiye.Alacak,
            Seviye = 1,
            FisKesilebilirMi = true,
            AktifMi = true
        };
        _db.HesapPlani.Add(hesap);
        return hesap;
    }

    private static MuhasebeFisiDetay YeniDetay(
        Guid fisId, Guid siteId, int sira, Guid hesapId, string hesapKodu, decimal borc, decimal alacak)
        => new()
        {
            FisId = fisId,
            SiteId = siteId,
            SiraNo = sira,
            HesapId = hesapId,
            HesapKodu = hesapKodu,
            BorcTutar = borc,
            AlacakTutar = alacak
        };

    private static MuhasebeFisi YeniFis(
        Guid id, Guid siteId, Guid donemId, int yevmiyeNo, string fisNo, FisTuru tur,
        DateOnly tarih, string aciklama, List<MuhasebeFisiDetay> detaylar)
        => new()
        {
            Id = id,
            SiteId = siteId,
            DonemId = donemId,
            YevmiyeNo = yevmiyeNo,
            FisNo = fisNo,
            FisTuru = tur,
            FisTarihi = tarih,
            Aciklama = aciklama,
            Durum = FisDurumu.Onaylandi,
            ToplamBorc = detaylar.Sum(d => d.BorcTutar),
            ToplamAlacak = detaylar.Sum(d => d.AlacakTutar),
            SatirSayisi = detaylar.Count,
            Detaylar = detaylar
        };
}
