using MediatR;
using Microsoft.Extensions.Logging;
using SiteYonetimi.Muhasebe.Hesaplar.Services;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Shared.Events;

namespace SiteYonetimi.Muhasebe.Integration;

/// <summary>
/// Kiracı/Ev sahibi eklendiğinde otomatik cari hesap açar. İdempotenttir
/// (CariHesapService PersonId üzerinden mükerrer açılışı engeller). Muhasebe
/// tarafındaki bir hata davet işlemini bozmaz; loglanır.
/// </summary>
public class PersonCreatedAccountingHandler : INotificationHandler<PersonCreatedDomainEvent>
{
    private readonly ICariHesapService _cariHesapService;
    private readonly ILogger<PersonCreatedAccountingHandler> _logger;

    public PersonCreatedAccountingHandler(
        ICariHesapService cariHesapService,
        ILogger<PersonCreatedAccountingHandler> logger)
    {
        _cariHesapService = cariHesapService;
        _logger = logger;
    }

    public async Task Handle(PersonCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        CariTuru? tur = notification.UserType switch
        {
            UserType.Owner => CariTuru.EvSahibi,
            UserType.Renter => CariTuru.Kiraci,
            _ => null
        };
        if (tur is null) return;

        var ad = $"{notification.FirstName} {notification.LastName}".Trim();

        try
        {
            var result = await _cariHesapService.EnsureCariHesapAsync(
                notification.SiteId, tur.Value, ad, notification.PersonId, null, cancellationToken);
            if (!result.IsSuccess)
                _logger.LogWarning("Otomatik cari hesap açılamadı (PersonId={PersonId}): {Error}",
                    notification.PersonId, result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Otomatik cari hesap açılırken hata (PersonId={PersonId}).", notification.PersonId);
        }
    }
}
