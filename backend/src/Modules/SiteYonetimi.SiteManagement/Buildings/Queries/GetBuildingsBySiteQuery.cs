using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Buildings.DTOs;

namespace SiteYonetimi.SiteManagement.Buildings.Queries;

public record GetBuildingsBySiteQuery(Guid SiteId) : IRequest<Result<List<BuildingSummaryDto>>>;

public class GetBuildingsBySiteQueryHandler : IRequestHandler<GetBuildingsBySiteQuery, Result<List<BuildingSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetBuildingsBySiteQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<BuildingSummaryDto>>> Handle(GetBuildingsBySiteQuery request, CancellationToken cancellationToken)
    {
        var buildings = await _db.Buildings
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = new List<BuildingSummaryDto>();
        foreach (var building in buildings)
        {
            int unitCount = 0;
            try { unitCount = await _db.Units.CountAsync(u => u.BuildingId == building.Id, cancellationToken); } catch { }

            result.Add(new BuildingSummaryDto(building.Id, building.Name, building.IsActive, unitCount));
        }

        return Result<List<BuildingSummaryDto>>.Success(result);
    }
}
