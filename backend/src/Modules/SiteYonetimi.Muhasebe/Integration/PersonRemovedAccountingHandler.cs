using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Shared.Events;

namespace SiteYonetimi.Muhasebe.Integration;

/// <summary>
/// Kişi siteden çıkarıldığında ilgili cari hesabı pasife alır (silmez —
/// hareket görmüş olabilir).
/// </summary>
public class PersonRemovedAccountingHandler : INotificationHandler<PersonRemovedFromSiteDomainEvent>
{
    private readonly SharedTenantDbContext _db;
    private readonly ILogger<PersonRemovedAccountingHandler> _logger;

    public PersonRemovedAccountingHandler(SharedTenantDbContext db, ILogger<PersonRemovedAccountingHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(PersonRemovedFromSiteDomainEvent notification, CancellationToken cancellationToken)
    {
        CariTuru? tur = notification.UserType switch
        {
            UserType.Owner => CariTuru.EvSahibi,
            UserType.Renter => CariTuru.Kiraci,
            _ => null
        };
        if (tur is null) return;

        try
        {
            var cari = await _db.HesapPlani.FirstOrDefaultAsync(
                h => h.SiteId == notification.SiteId
                    && h.PersonId == notification.PersonId
                    && h.CariTuru == tur,
                cancellationToken);

            if (cari is not null && cari.AktifMi)
            {
                cari.AktifMi = false;
                cari.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cari hesap pasife alınırken hata (PersonId={PersonId}).", notification.PersonId);
        }
    }
}
