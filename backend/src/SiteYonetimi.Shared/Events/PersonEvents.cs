using MediatR;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Shared.Events;

/// <summary>
/// Bir kişi (User) bir siteye Owner/Renter olarak eklendiğinde yayınlanır.
/// Muhasebe modülü bunu dinleyip otomatik cari hesap açar (idempotent).
/// </summary>
public record PersonCreatedDomainEvent(
    Guid SiteId,
    Guid PersonId,
    string FirstName,
    string LastName,
    UserType UserType) : INotification;

/// <summary>
/// Bir kişinin siteyle ilişkisi kaldırıldığında/pasife alındığında yayınlanır.
/// Muhasebe modülü ilgili cari hesabı pasife alır (silmez).
/// </summary>
public record PersonRemovedFromSiteDomainEvent(
    Guid SiteId,
    Guid PersonId,
    UserType UserType) : INotification;
