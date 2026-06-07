using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record UpdatePaymentStatusCommand(Guid Id, Guid SiteId, UpdatePaymentStatusDto Dto) : IRequest<Result>;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePaymentStatusCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Ödeme bulunamadı.");

        entity.Status = request.Dto.Status;
        entity.PaidDate = request.Dto.PaidDate;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
