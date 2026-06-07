using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record DeletePaymentCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePaymentCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Ödeme bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
