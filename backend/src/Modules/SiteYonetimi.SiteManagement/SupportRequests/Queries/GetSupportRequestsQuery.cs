using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;

namespace SiteYonetimi.SiteManagement.SupportRequests.Queries;

public record GetSupportRequestsBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<SupportRequestSummaryDto>>>;
public record GetSupportRequestsByUserQuery(Guid UserId, Guid SiteId) : IRequest<Result<List<SupportRequestSummaryDto>>>;
public record GetSupportRequestByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<SupportRequestDetailDto>>;

public class GetSupportRequestsBySiteQueryHandler : IRequestHandler<GetSupportRequestsBySiteQuery, Result<PaginatedResult<SupportRequestSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetSupportRequestsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<SupportRequestSummaryDto>>> Handle(GetSupportRequestsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.SupportRequests
            .Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x => x.Subject.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SupportRequestSummaryDto(
                x.Id, x.SiteId, x.UserId, x.UnitId,
                x.Unit.DoorNumber, x.Subject, x.Status, x.CreatedAt, x.ResolvedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<SupportRequestSummaryDto>>.Success(
            PaginatedResult<SupportRequestSummaryDto>.Create(items, totalCount, request.Page, request.PageSize));
    }
}

public class GetSupportRequestsByUserQueryHandler : IRequestHandler<GetSupportRequestsByUserQuery, Result<List<SupportRequestSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetSupportRequestsByUserQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<SupportRequestSummaryDto>>> Handle(GetSupportRequestsByUserQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.SupportRequests
            .Where(x => x.SiteId == request.SiteId && x.UserId == request.UserId)
            .Include(x => x.Unit)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SupportRequestSummaryDto(
                x.Id, x.SiteId, x.UserId, x.UnitId,
                x.Unit.DoorNumber, x.Subject, x.Status, x.CreatedAt, x.ResolvedAt))
            .ToListAsync(cancellationToken);

        return Result<List<SupportRequestSummaryDto>>.Success(items);
    }
}

public class GetSupportRequestByIdQueryHandler : IRequestHandler<GetSupportRequestByIdQuery, Result<SupportRequestDetailDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetSupportRequestByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<SupportRequestDetailDto>> Handle(GetSupportRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.SupportRequests
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<SupportRequestDetailDto>.Failure("Destek talebi bulunamadı.");

        return Result<SupportRequestDetailDto>.Success(new SupportRequestDetailDto(
            item.Id, item.SiteId, item.UserId, item.UnitId,
            item.Unit?.DoorNumber, item.Subject, item.Description,
            item.Status, item.CreatedAt, item.UpdatedAt, item.ResolvedAt));
    }
}
