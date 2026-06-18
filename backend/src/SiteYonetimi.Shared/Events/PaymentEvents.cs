using MediatR;

namespace SiteYonetimi.Shared.Events;

/// <summary>
/// Bir ödeme (aidat) "ödendi" durumuna geçtiğinde yayınlanır. Muhasebe modülü,
/// parametre açıksa otomatik Tahsil fişi üretir (Borç Kasa/Banka, Alacak cari).
/// </summary>
public record PaymentPaidDomainEvent(
    Guid SiteId,
    Guid PaymentId,
    Guid UnitId,
    decimal Amount,
    DateOnly PaidDate) : INotification;
