using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Queries;

public record GetUnitsBySiteQuery(Guid SiteId, Guid? BuildingId = null) : IRequest<Result<List<UnitSummaryDto>>>;

public class GetUnitsBySiteQueryHandler : IRequestHandler<GetUnitsBySiteQuery, Result<List<UnitSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetUnitsBySiteQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<UnitSummaryDto>>> Handle(GetUnitsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .Where(x => x.SiteId == request.SiteId);

        if (request.BuildingId.HasValue)
            query = query.Where(x => x.BuildingId == request.BuildingId.Value);

        var units = await query
            .OrderBy(x => x.Building.Name)
            .ThenBy(x => x.DoorNumber)
            .ToListAsync(cancellationToken);

        var result = units.Select(u => new UnitSummaryDto(
            u.Id,
            u.BuildingId,
            u.Building?.Name,
            u.UnitTypeId,
            u.UnitType?.Name,
            u.DoorNumber,
            u.Code,
            u.Floor,
            u.Status,
            u.MonthlyFee,
            u.HasDask,
            u.CreatedAt
        )).ToList();

        return Result<List<UnitSummaryDto>>.Success(result);
    }
}
