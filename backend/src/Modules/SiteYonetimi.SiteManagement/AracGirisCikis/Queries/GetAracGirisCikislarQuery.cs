using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AracGirisCikis.DTOs;

namespace SiteYonetimi.SiteManagement.AracGirisCikis.Queries;

public record GetAracGirisCikislarQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<AracGirisCikisDto>>>;

public class GetAracGirisCikislarQueryHandler
    : IRequestHandler<GetAracGirisCikislarQuery, Result<PaginatedResult<AracGirisCikisDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAracGirisCikislarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<AracGirisCikisDto>>> Handle(GetAracGirisCikislarQuery request, CancellationToken ct)
    {
        var q = _db.AracGirisCikislar.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Plaka.ToLower().Contains(s) || (x.SuruculAdi != null && x.SuruculAdi.ToLower().Contains(s)));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.GirisSaati)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AracGirisCikisDto(
                x.Id, x.Plaka, x.SuruculAdi,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.AracTipi, x.GirisSaati, x.CikisSaati, x.Aciklama, x.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedResult<AracGirisCikisDto>>.Success(
            new PaginatedResult<AracGirisCikisDto>(items, total, request.Page, request.PageSize));
    }
}
