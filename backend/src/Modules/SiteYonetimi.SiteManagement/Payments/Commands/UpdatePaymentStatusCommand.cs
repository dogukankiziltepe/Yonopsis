using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Shared.Events;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record UpdatePaymentStatusCommand(Guid Id, Guid SiteId, UpdatePaymentStatusDto Dto) : IRequest<Result>;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IPublisher _publisher;

    public UpdatePaymentStatusCommandHandler(SharedTenantDbContext db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<Result> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Ödeme bulunamadı.");

        var oncekiDurum = entity.Status;

        entity.Status = request.Dto.Status;
        entity.PaidDate = request.Dto.PaidDate;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        // "Ödendi" durumuna ilk geçişte otomatik tahsil fişi (Muhasebe modülü, parametre kontrollü).
        if (request.Dto.Status == PaymentStatus.Paid && oncekiDurum != PaymentStatus.Paid)
        {
            var paid = entity.PaidDate ?? DateTime.UtcNow;
            await _publisher.Publish(
                new PaymentPaidDomainEvent(entity.SiteId, entity.Id, entity.UnitId, entity.Amount,
                    DateOnly.FromDateTime(paid)),
                cancellationToken);
        }

        return Result.Success();
    }
}
