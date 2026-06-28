using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirGruplari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirGruplari.Queries;

public record GetGelirGruplariQuery(Guid SiteId) : IRequest<Result<List<GelirGrubuDto>>>;

public class GetGelirGruplariQueryHandler : IRequestHandler<GetGelirGruplariQuery, Result<List<GelirGrubuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetGelirGruplariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<GelirGrubuDto>>> Handle(GetGelirGruplariQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.GelirGruplari
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new GelirGrubuDto(x.Id, x.Name, x.Description, x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<GelirGrubuDto>>.Success(items);
    }
}
