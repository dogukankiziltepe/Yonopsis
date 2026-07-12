using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GiderTanimlari.DTOs;

namespace SiteYonetimi.SiteManagement.GiderTanimlari.Queries;

public record GetGiderTanimlariQuery(Guid SiteId) : IRequest<Result<List<GiderTanimiDto>>>;

public class GetGiderTanimlariQueryHandler : IRequestHandler<GetGiderTanimlariQuery, Result<List<GiderTanimiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetGiderTanimlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<GiderTanimiDto>>> Handle(GetGiderTanimlariQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.GiderTanimlari
            .Include(x => x.GiderGrubu)
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new GiderTanimiDto(
                x.Id, x.GiderKodu, x.Name, x.Description,
                x.GiderGrubuId, x.GiderGrubu != null ? x.GiderGrubu.Name : null,
                x.DagitimSekli, x.BosDairelereDagit, x.Kdv, x.BorclandirilacakKisi, x.MuhasebeKodu,
                x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<GiderTanimiDto>>.Success(items);
    }
}
