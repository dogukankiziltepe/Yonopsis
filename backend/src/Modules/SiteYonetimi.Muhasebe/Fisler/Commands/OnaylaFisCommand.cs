using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Commands;

/// <summary>
/// Fişi onaylar (entegre eder). Denge kontrolü yapar ve dönemin yevmiye
/// sayacını atomik artırarak sıralı (boşluksuz) yevmiye no atar.
/// </summary>
public record OnaylaFisCommand(Guid SiteId, Guid Id) : IRequest<Result>;

public class OnaylaFisCommandHandler : IRequestHandler<OnaylaFisCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public OnaylaFisCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(OnaylaFisCommand request, CancellationToken cancellationToken)
    {
        var fis = await _db.MuhasebeFisleri
            .FirstOrDefaultAsync(f => f.SiteId == request.SiteId && f.Id == request.Id, cancellationToken);
        if (fis is null)
            return Result.Failure("Fiş bulunamadı.");
        if (fis.Durum != FisDurumu.Taslak)
            return Result.Failure("Yalnızca taslak fişler onaylanabilir.");
        if (fis.SatirSayisi == 0)
            return Result.Failure("Boş fiş onaylanamaz.");
        if (fis.ToplamBorc <= 0 || fis.ToplamBorc != fis.ToplamAlacak)
            return Result.Failure("Fiş dengesiz: toplam borç ile toplam alacak eşit ve sıfırdan büyük olmalı.");

        // Yevmiye no atomik artış için transaction + dönem üzerinde kilit.
        await using var tx = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);
        try
        {
            var donem = await _db.MuhasebeDonemler
                .FirstOrDefaultAsync(d => d.Id == fis.DonemId, cancellationToken);
            if (donem is null)
                return Result.Failure("Dönem bulunamadı.");
            if (donem.Durum == DonemDurumu.Kapali)
                return Result.Failure("Dönem kapalı; fiş onaylanamaz.");

            donem.SonYevmiyeNo += 1;
            donem.UpdatedAt = DateTime.UtcNow;

            fis.YevmiyeNo = donem.SonYevmiyeNo;
            fis.Durum = FisDurumu.Onaylandi;
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
