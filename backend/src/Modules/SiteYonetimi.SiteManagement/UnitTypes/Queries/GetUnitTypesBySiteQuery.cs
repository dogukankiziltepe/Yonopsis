using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.UnitTypes.DTOs;

namespace SiteYonetimi.SiteManagement.UnitTypes.Queries;

public record GetUnitTypesBySiteQuery(Guid SiteId) : IRequest<Result<List<UnitTypeDto>>>;

public class GetUnitTypesBySiteQueryHandler : IRequestHandler<GetUnitTypesBySiteQuery, Result<List<UnitTypeDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetUnitTypesBySiteQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<UnitTypeDto>>> Handle(GetUnitTypesBySiteQuery request, CancellationToken cancellationToken)
    {
        var unitTypes = await _db.UnitTypes
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Name)
            .Select(x => new UnitTypeDto(x.Id, x.SiteId, x.Name, x.IsActive, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<UnitTypeDto>>.Success(unitTypes);
    }
}
