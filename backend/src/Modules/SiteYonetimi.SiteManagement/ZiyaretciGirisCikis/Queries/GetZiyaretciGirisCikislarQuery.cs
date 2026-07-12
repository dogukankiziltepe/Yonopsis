using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.DTOs;

namespace SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.Queries;

public record GetZiyaretciGirisCikislarQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<ZiyaretciGirisCikisDto>>>;

public class GetZiyaretciGirisCikislarQueryHandler
    : IRequestHandler<GetZiyaretciGirisCikislarQuery, Result<PaginatedResult<ZiyaretciGirisCikisDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetZiyaretciGirisCikislarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<ZiyaretciGirisCikisDto>>> Handle(
        GetZiyaretciGirisCikislarQuery request, CancellationToken ct)
    {
        var q = _db.ZiyaretciGirisCikislar.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.GelensAdi.ToLower().Contains(s)
                || (x.Plaka != null && x.Plaka.ToLower().Contains(s))
                || (x.GeldigiKisi != null && x.GeldigiKisi.ToLower().Contains(s)));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.GirisSaati)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ZiyaretciGirisCikisDto(
                x.Id, x.GelensAdi, x.GeldigiKisi,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.ZiyaretAmaci, x.GirisSaati, x.CikisSaati, x.Plaka, x.Aciklama, x.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedResult<ZiyaretciGirisCikisDto>>.Success(
            new PaginatedResult<ZiyaretciGirisCikisDto>(items, total, request.Page, request.PageSize));
    }
}
