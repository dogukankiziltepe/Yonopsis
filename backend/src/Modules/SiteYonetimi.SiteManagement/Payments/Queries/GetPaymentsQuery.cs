using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Queries;

public record GetPaymentsBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<PaymentSummaryDto>>>;
public record GetPaymentsByUnitQuery(Guid UnitId, Guid SiteId) : IRequest<Result<List<PaymentSummaryDto>>>;
public record GetPaymentByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<PaymentSummaryDto>>;

public class GetPaymentsBySiteQueryHandler : IRequestHandler<GetPaymentsBySiteQuery, Result<PaginatedResult<PaymentSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<PaymentSummaryDto>>> Handle(GetPaymentsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Payments
            .Include(x => x.Unit)
            .Include(x => x.Items)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                (x.Description != null && x.Description.ToLower().Contains(term)) ||
                x.Unit.DoorNumber.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var raw = await query
            .OrderByDescending(x => x.DueDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = raw.Select(x => new PaymentSummaryDto(
            x.Id, x.SiteId, x.UnitId, x.Unit.DoorNumber,
            x.Amount,
            x.Items.Select(i => new PaymentItemDto(i.Name, i.Amount)).ToList(),
            x.DueDate, x.PaidDate, x.Status, x.Description, x.CreatedAt))
            .ToList();

        return Result<PaginatedResult<PaymentSummaryDto>>.Success(
            PaginatedResult<PaymentSummaryDto>.Create(items, totalCount, request.Page, request.PageSize));
    }
}

public class GetPaymentsByUnitQueryHandler : IRequestHandler<GetPaymentsByUnitQuery, Result<List<PaymentSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentsByUnitQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<PaymentSummaryDto>>> Handle(GetPaymentsByUnitQuery request, CancellationToken cancellationToken)
    {
        var raw = await _db.Payments
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId)
            .Include(x => x.Unit)
            .Include(x => x.Items)
            .OrderByDescending(x => x.DueDate)
            .ToListAsync(cancellationToken);

        var items = raw.Select(x => new PaymentSummaryDto(
            x.Id, x.SiteId, x.UnitId, x.Unit.DoorNumber,
            x.Amount,
            x.Items.Select(i => new PaymentItemDto(i.Name, i.Amount)).ToList(),
            x.DueDate, x.PaidDate, x.Status, x.Description, x.CreatedAt))
            .ToList();

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
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<PaymentSummaryDto>.Failure("Ödeme bulunamadı.");

        return Result<PaymentSummaryDto>.Success(new PaymentSummaryDto(
            item.Id, item.SiteId, item.UnitId, item.Unit?.DoorNumber,
            item.Amount,
            item.Items.Select(i => new PaymentItemDto(i.Name, i.Amount)).ToList(),
            item.DueDate, item.PaidDate, item.Status, item.Description, item.CreatedAt));
    }
}
