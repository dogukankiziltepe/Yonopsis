using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record MarkOverduePaymentsCommand(Guid SiteId) : IRequest<Result<int>>;

public class MarkOverduePaymentsCommandHandler : IRequestHandler<MarkOverduePaymentsCommand, Result<int>>
{
    private readonly SharedTenantDbContext _db;
    public MarkOverduePaymentsCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<int>> Handle(MarkOverduePaymentsCommand request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var overdueList = await _db.Payments
            .Where(p => p.SiteId == request.SiteId
                && !p.IsDeleted
                && p.Status == PaymentStatus.Pending
                && p.DueDate < today)
            .ToListAsync(cancellationToken);

        foreach (var p in overdueList)
        {
            p.Status = PaymentStatus.Overdue;
            p.UpdatedAt = DateTime.UtcNow;
        }

        if (overdueList.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(overdueList.Count);
    }
}
