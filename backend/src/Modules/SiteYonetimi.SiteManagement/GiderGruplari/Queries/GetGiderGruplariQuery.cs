using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GiderGruplari.DTOs;

namespace SiteYonetimi.SiteManagement.GiderGruplari.Queries;

public record GetGiderGruplariQuery(Guid SiteId) : IRequest<Result<List<GiderGrubuDto>>>;

public class GetGiderGruplariQueryHandler : IRequestHandler<GetGiderGruplariQuery, Result<List<GiderGrubuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetGiderGruplariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<GiderGrubuDto>>> Handle(GetGiderGruplariQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.GiderGruplari
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new GiderGrubuDto(x.Id, x.Name, x.Description, x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<GiderGrubuDto>>.Success(items);
    }
}
