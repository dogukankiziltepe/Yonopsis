using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AccessCards.DTOs;

namespace SiteYonetimi.SiteManagement.AccessCards.Queries;

public record GetAccessCardsBySiteQuery(Guid SiteId) : IRequest<Result<List<AccessCardSummaryDto>>>;
public record GetAccessCardsByUserQuery(Guid UserId, Guid SiteId) : IRequest<Result<List<AccessCardSummaryDto>>>;
public record GetAccessCardByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<AccessCardSummaryDto>>;

public class GetAccessCardsBySiteQueryHandler : IRequestHandler<GetAccessCardsBySiteQuery, Result<List<AccessCardSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAccessCardsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AccessCardSummaryDto>>> Handle(GetAccessCardsBySiteQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.AccessCards
            .Where(x => x.SiteId == request.SiteId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AccessCardSummaryDto(
                x.Id, x.SiteId, x.UserId, x.CardNumber,
                x.IsActive, x.IssueDate, x.ExpiryDate, x.Notes, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<AccessCardSummaryDto>>.Success(items);
    }
}

public class GetAccessCardsByUserQueryHandler : IRequestHandler<GetAccessCardsByUserQuery, Result<List<AccessCardSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAccessCardsByUserQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AccessCardSummaryDto>>> Handle(GetAccessCardsByUserQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.AccessCards
            .Where(x => x.SiteId == request.SiteId && x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AccessCardSummaryDto(
                x.Id, x.SiteId, x.UserId, x.CardNumber,
                x.IsActive, x.IssueDate, x.ExpiryDate, x.Notes, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<AccessCardSummaryDto>>.Success(items);
    }
}

public class GetAccessCardByIdQueryHandler : IRequestHandler<GetAccessCardByIdQuery, Result<AccessCardSummaryDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetAccessCardByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<AccessCardSummaryDto>> Handle(GetAccessCardByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.AccessCards
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<AccessCardSummaryDto>.Failure("Kart bulunamadı.");

        return Result<AccessCardSummaryDto>.Success(new AccessCardSummaryDto(
            item.Id, item.SiteId, item.UserId, item.CardNumber,
            item.IsActive, item.IssueDate, item.ExpiryDate, item.Notes, item.CreatedAt));
    }
}
