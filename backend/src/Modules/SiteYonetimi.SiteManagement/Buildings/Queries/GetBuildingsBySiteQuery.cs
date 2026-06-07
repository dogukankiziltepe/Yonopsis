using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Buildings.DTOs;

namespace SiteYonetimi.SiteManagement.Buildings.Queries;

public record GetBuildingsBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<BuildingSummaryDto>>>;

public class GetBuildingsBySiteQueryHandler : IRequestHandler<GetBuildingsBySiteQuery, Result<PaginatedResult<BuildingSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetBuildingsBySiteQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResult<BuildingSummaryDto>>> Handle(GetBuildingsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Buildings.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(x => x.Name.Contains(request.SearchTerm));

        var totalCount = await query.CountAsync(cancellationToken);

        var buildings = await query
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var buildingIds = buildings.Select(b => b.Id).ToList();
        var unitCounts = await _db.Units
            .Where(u => buildingIds.Contains(u.BuildingId))
            .GroupBy(u => u.BuildingId)
            .Select(g => new { BuildingId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BuildingId, x => x.Count, cancellationToken);

        var result = buildings.Select(b =>
            new BuildingSummaryDto(b.Id, b.Name, b.IsActive, unitCounts.GetValueOrDefault(b.Id, 0))).ToList();

        return Result<PaginatedResult<BuildingSummaryDto>>.Success(
            PaginatedResult<BuildingSummaryDto>.Create(result, totalCount, request.Page, request.PageSize));
    }
}
