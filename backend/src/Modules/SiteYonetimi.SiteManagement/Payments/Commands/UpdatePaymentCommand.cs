using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record UpdatePaymentCommand(Guid Id, Guid SiteId, UpdatePaymentDto Dto) : IRequest<Result>;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePaymentCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Ödeme bulunamadı.");

        entity.Amount = request.Dto.Amount;
        entity.DueDate = request.Dto.DueDate;
        entity.Description = request.Dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
