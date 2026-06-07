using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Queries;

public record GetPaymentsBySiteQuery(Guid SiteId) : IRequest<Result<List<PaymentSummaryDto>>>;
public record GetPaymentsByUnitQuery(Guid UnitId, Guid SiteId) : IRequest<Result<List<PaymentSummaryDto>>>;
public record GetPaymentByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<PaymentSummaryDto>>;

public class GetPaymentsBySiteQueryHandler : IRequestHandler<GetPaymentsBySiteQuery, Result<List<PaymentSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<PaymentSummaryDto>>> Handle(GetPaymentsBySiteQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Payments
            .Where(x => x.SiteId == request.SiteId)
            .Include(x => x.Unit)
            .OrderByDescending(x => x.DueDate)
            .Select(x => new PaymentSummaryDto(
                x.Id, x.SiteId, x.UnitId, x.Unit.DoorNumber,
                x.Amount, x.DueDate, x.PaidDate, x.Status, x.Description, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<PaymentSummaryDto>>.Success(items);
    }
}

public class GetPaymentsByUnitQueryHandler : IRequestHandler<GetPaymentsByUnitQuery, Result<List<PaymentSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentsByUnitQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<PaymentSummaryDto>>> Handle(GetPaymentsByUnitQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Payments
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId)
            .Include(x => x.Unit)
            .OrderByDescending(x => x.DueDate)
            .Select(x => new PaymentSummaryDto(
                x.Id, x.SiteId, x.UnitId, x.Unit.DoorNumber,
                x.Amount, x.DueDate, x.PaidDate, x.Status, x.Description, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<PaymentSummaryDto>>.Success(items);
    }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentSummaryDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaymentSummaryDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.Payments
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<PaymentSummaryDto>.Failure("Ödeme bulunamadı.");

        return Result<PaymentSummaryDto>.Success(new PaymentSummaryDto(
            item.Id, item.SiteId, item.UnitId, item.Unit?.DoorNumber,
            item.Amount, item.DueDate, item.PaidDate, item.Status, item.Description, item.CreatedAt));
    }
}
