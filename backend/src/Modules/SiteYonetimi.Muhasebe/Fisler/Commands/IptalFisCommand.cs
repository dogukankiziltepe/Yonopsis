using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Commands;

/// <summary>
/// Fişi iptal eder. Taslak fiş doğrudan iptal edilir. Onaylı fiş için ters
/// kayıt (storno) fişi üretilir: borç/alacak yer değiştirir, storno onaylanır
/// (yevmiye no alır) ve orijinal fiş Iptal durumuna alınır.
/// </summary>
public record IptalFisCommand(Guid SiteId, Guid Id) : IRequest<Result>;

public class IptalFisCommandHandler : IRequestHandler<IptalFisCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IFisService _fisService;

    public IptalFisCommandHandler(SharedTenantDbContext db, IFisService fisService)
    {
        _db = db;
        _fisService = fisService;
    }

    public async Task<Result> Handle(IptalFisCommand request, CancellationToken cancellationToken)
    {
        var fis = await _db.MuhasebeFisleri
            .Include(f => f.Detaylar)
            .FirstOrDefaultAsync(f => f.SiteId == request.SiteId && f.Id == request.Id, cancellationToken);
        if (fis is null)
            return Result.Failure("Fiş bulunamadı.");
        if (fis.Durum == FisDurumu.Iptal)
            return Result.Failure("Fiş zaten iptal edilmiş.");

        // Taslak fiş: doğrudan iptal.
        if (fis.Durum == FisDurumu.Taslak)
        {
            fis.Durum = FisDurumu.Iptal;
            fis.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        // Onaylı fiş: ters kayıt üret.
        await using var tx = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            var donem = await _db.MuhasebeDonemler
                .FirstOrDefaultAsync(d => d.Id == fis.DonemId, cancellationToken);
            if (donem is null)
                return Result.Failure("Dönem bulunamadı.");
            if (donem.Durum == DonemDurumu.Kapali)
                return Result.Failure("Dönem kapalı; iptal/storno yapılamaz.");

            var stornoId = Guid.NewGuid();
            var stornoDetaylar = fis.Detaylar
                .OrderBy(d => d.SiraNo)
                .Select((d, i) => new MuhasebeFisiDetay
                {
                    FisId = stornoId,
                    SiteId = request.SiteId,
                    SiraNo = i + 1,
                    HesapId = d.HesapId,
                    HesapKodu = d.HesapKodu,
                    // Ters kayıt: borç/alacak yer değiştirir.
                    BorcTutar = d.AlacakTutar,
                    AlacakTutar = d.BorcTutar,
                    Aciklama = d.Aciklama,
                    BelgeNo = d.BelgeNo
                })
                .ToList();

            var fisNo = await _fisService.GenerateFisNoAsync(request.SiteId, donem.Yil, cancellationToken);

            donem.SonYevmiyeNo += 1;
            donem.UpdatedAt = DateTime.UtcNow;

            var storno = new MuhasebeFisi
            {
                Id = stornoId,
                SiteId = request.SiteId,
                DonemId = donem.Id,
                YevmiyeNo = donem.SonYevmiyeNo,
                FisNo = fisNo,
                FisTuru = FisTuru.Mahsup,
                FisTarihi = DateOnly.FromDateTime(DateTime.UtcNow),
                Aciklama = $"STORNO: {fis.FisNo} - {fis.Aciklama}".Trim(),
                Durum = FisDurumu.Onaylandi,
                ToplamBorc = stornoDetaylar.Sum(d => d.BorcTutar),
                ToplamAlacak = stornoDetaylar.Sum(d => d.AlacakTutar),
                SatirSayisi = stornoDetaylar.Count,
                Detaylar = stornoDetaylar
            };
            _db.MuhasebeFisleri.Add(storno);

            fis.Durum = FisDurumu.Iptal;
            fis.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
