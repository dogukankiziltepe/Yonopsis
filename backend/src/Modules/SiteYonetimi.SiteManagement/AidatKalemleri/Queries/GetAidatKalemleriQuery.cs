using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AidatKalemleri.DTOs;

namespace SiteYonetimi.SiteManagement.AidatKalemleri.Queries;

public record GetAidatKalemleriQuery(Guid SiteId) : IRequest<Result<List<AidatKalemiDto>>>;

public class GetAidatKalemleriQueryHandler : IRequestHandler<GetAidatKalemleriQuery, Result<List<AidatKalemiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAidatKalemleriQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AidatKalemiDto>>> Handle(GetAidatKalemleriQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.AidatKalemleri
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .Select(x => new AidatKalemiDto(x.Id, x.SiteId, x.Name, x.Description, x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<AidatKalemiDto>>.Success(items);
    }
}
