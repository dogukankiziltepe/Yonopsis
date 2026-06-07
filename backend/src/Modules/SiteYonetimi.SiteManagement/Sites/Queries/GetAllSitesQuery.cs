using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Sites.DTOs;

namespace SiteYonetimi.SiteManagement.Sites.Queries;

public record GetAllSitesQuery : IRequest<Result<List<SiteSummaryDto>>>;

public class GetAllSitesQueryHandler : IRequestHandler<GetAllSitesQuery, Result<List<SiteSummaryDto>>>
{
    private readonly MasterDbContext _db;
    private readonly SharedTenantDbContext _sharedDb;

    public GetAllSitesQueryHandler(MasterDbContext db, SharedTenantDbContext sharedDb)
    {
        _db = db;
        _sharedDb = sharedDb;
    }

    public async Task<Result<List<SiteSummaryDto>>> Handle(GetAllSitesQuery request, CancellationToken cancellationToken)
    {
        var sites = await _db.Sites
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = new List<SiteSummaryDto>();
        foreach (var site in sites)
        {
            int blockCount = 0;
            int unitCount = 0;

            try { blockCount = await _sharedDb.Buildings.CountAsync(b => b.SiteId == site.Id, cancellationToken); } catch { }
            try { unitCount = await _sharedDb.Units.CountAsync(u => u.SiteId == site.Id, cancellationToken); } catch { }

            result.Add(new SiteSummaryDto(site.Id, site.Name, site.City, site.District, site.IsActive, blockCount, unitCount));
        }

        return Result<List<SiteSummaryDto>>.Success(result);
    }
}
