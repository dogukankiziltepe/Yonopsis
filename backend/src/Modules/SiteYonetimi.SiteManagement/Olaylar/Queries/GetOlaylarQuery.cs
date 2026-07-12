using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Olaylar.DTOs;

namespace SiteYonetimi.SiteManagement.Olaylar.Queries;

public record GetOlaylarQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null, OlayDurum? Durum = null)
    : IRequest<Result<PaginatedResult<OlayDto>>>;

public class GetOlaylarQueryHandler : IRequestHandler<GetOlaylarQuery, Result<PaginatedResult<OlayDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetOlaylarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<OlayDto>>> Handle(GetOlaylarQuery request, CancellationToken ct)
    {
        var q = _db.Olaylar.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Baslik.ToLower().Contains(s) || (x.Konum != null && x.Konum.ToLower().Contains(s)));
        }

        if (request.Durum.HasValue)
            q = q.Where(x => x.Durum == request.Durum.Value);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.OlayTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new OlayDto(
                x.Id, x.Baslik, x.Aciklama, x.OlayTarihi, x.Tip, x.Konum,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null, x.Durum, x.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedResult<OlayDto>>.Success(
            new PaginatedResult<OlayDto>(items, total, request.Page, request.PageSize));
    }
}
