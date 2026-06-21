using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Hesaplar.Commands;

/// <summary>
/// Hesabı aktif/pasif yapar. Hareket görmüş hesaplar silinmez, pasife alınır
/// (bkz. iş kuralı §4.8).
/// </summary>
public record ToggleHesapAktifCommand(Guid SiteId, Guid Id, bool Aktif) : IRequest<Result>;

public class ToggleHesapAktifCommandHandler : IRequestHandler<ToggleHesapAktifCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public ToggleHesapAktifCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(ToggleHesapAktifCommand request, CancellationToken cancellationToken)
    {
        var hesap = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == request.SiteId && h.Id == request.Id, cancellationToken);
        if (hesap is null)
            return Result.Failure("Hesap bulunamadı.");

        hesap.AktifMi = request.Aktif;
        hesap.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
