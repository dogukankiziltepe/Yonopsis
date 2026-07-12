using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.YapilacakIsler.DTOs;

namespace SiteYonetimi.SiteManagement.YapilacakIsler.Queries;

public record GetYapilacakIslerQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, YapilacakIsDurum? Durum = null)
    : IRequest<Result<PaginatedResult<YapilacakIsDto>>>;

public class GetYapilacakIslerQueryHandler : IRequestHandler<GetYapilacakIslerQuery, Result<PaginatedResult<YapilacakIsDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetYapilacakIslerQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<YapilacakIsDto>>> Handle(GetYapilacakIslerQuery request, CancellationToken ct)
    {
        var q = _db.YapilacakIsler.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s)); }
        if (request.Durum.HasValue) q = q.Where(x => x.Durum == request.Durum.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Oncelik).ThenBy(x => x.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new YapilacakIsDto(x.Id, x.Baslik, x.Aciklama, x.AtananKisi, x.Oncelik, x.TamamlanmaTarihi, x.Durum, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<YapilacakIsDto>>.Success(new PaginatedResult<YapilacakIsDto>(items, total, request.Page, request.PageSize));
    }
}
