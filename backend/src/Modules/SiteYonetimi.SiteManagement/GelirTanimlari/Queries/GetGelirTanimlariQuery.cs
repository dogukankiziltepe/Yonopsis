using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirTanimlari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirTanimlari.Queries;

public record GetGelirTanimlariQuery(Guid SiteId) : IRequest<Result<List<GelirTanimiDto>>>;

public class GetGelirTanimlariQueryHandler : IRequestHandler<GetGelirTanimlariQuery, Result<List<GelirTanimiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetGelirTanimlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<GelirTanimiDto>>> Handle(GetGelirTanimlariQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.GelirTanimlari
            .Include(x => x.GelirGrubu)
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new GelirTanimiDto(
                x.Id, x.Name, x.Description,
                x.GelirGrubuId, x.GelirGrubu != null ? x.GelirGrubu.Name : null,
                x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<GelirTanimiDto>>.Success(items);
    }
}
