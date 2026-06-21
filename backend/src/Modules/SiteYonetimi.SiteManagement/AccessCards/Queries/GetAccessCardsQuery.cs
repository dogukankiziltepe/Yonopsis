using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AccessCards.DTOs;

namespace SiteYonetimi.SiteManagement.AccessCards.Queries;

public record GetAccessCardsBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<AccessCardSummaryDto>>>;
public record GetAccessCardsByUserQuery(Guid UserId, Guid SiteId) : IRequest<Result<List<AccessCardSummaryDto>>>;
public record GetAccessCardByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<AccessCardSummaryDto>>;

public class GetAccessCardsBySiteQueryHandler : IRequestHandler<GetAccessCardsBySiteQuery, Result<PaginatedResult<AccessCardSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetAccessCardsBySiteQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<PaginatedResult<AccessCardSummaryDto>>> Handle(GetAccessCardsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.AccessCards
            .Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.CardNumber.ToLower().Contains(term) ||
                (x.Notes != null && x.Notes.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var cards = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userIds = cards.Select(x => x.UserId).Distinct().ToList();
        var users = await _masterDb.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToListAsync(cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        var items = cards.Select(x => new AccessCardSummaryDto(
            x.Id,
            x.SiteId,
            x.UserId,
            userMap.GetValueOrDefault(x.UserId),
            x.UnitId,
            x.Unit?.DoorNumber,
            x.CardNumber,
            x.IsActive,
            x.IssueDate,
            x.ExpiryDate,
            x.Notes,
            x.CreatedAt)).ToList();

        return Result<PaginatedResult<AccessCardSummaryDto>>.Success(
            PaginatedResult<AccessCardSummaryDto>.Create(items, totalCount, request.Page, request.PageSize));
    }
}

public class GetAccessCardsByUserQueryHandler : IRequestHandler<GetAccessCardsByUserQuery, Result<List<AccessCardSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetAccessCardsByUserQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<List<AccessCardSummaryDto>>> Handle(GetAccessCardsByUserQuery request, CancellationToken cancellationToken)
    {
        var cards = await _db.AccessCards
            .Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId && x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var person = await _masterDb.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new { u.FirstName, u.LastName })
            .FirstOrDefaultAsync(cancellationToken);
        var personFullName = person is not null ? $"{person.FirstName} {person.LastName}" : null;

        var items = cards.Select(x => new AccessCardSummaryDto(
            x.Id,
            x.SiteId,
            x.UserId,
            personFullName,
            x.UnitId,
            x.Unit?.DoorNumber,
            x.CardNumber,
            x.IsActive,
            x.IssueDate,
            x.ExpiryDate,
            x.Notes,
            x.CreatedAt)).ToList();

        return Result<List<AccessCardSummaryDto>>.Success(items);
    }
}

public class GetAccessCardByIdQueryHandler : IRequestHandler<GetAccessCardByIdQuery, Result<AccessCardSummaryDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetAccessCardByIdQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<AccessCardSummaryDto>> Handle(GetAccessCardByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.AccessCards
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<AccessCardSummaryDto>.Failure("Kart bulunamadı.");

        var person = await _masterDb.Users
            .Where(u => u.Id == item.UserId)
            .Select(u => new { u.FirstName, u.LastName })
            .FirstOrDefaultAsync(cancellationToken);
        var personFullName = person is not null ? $"{person.FirstName} {person.LastName}" : null;

        return Result<AccessCardSummaryDto>.Success(new AccessCardSummaryDto(
            item.Id,
            item.SiteId,
            item.UserId,
            personFullName,
            item.UnitId,
            item.Unit?.DoorNumber,
            item.CardNumber,
            item.IsActive,
            item.IssueDate,
            item.ExpiryDate,
            item.Notes,
            item.CreatedAt));
    }
}
