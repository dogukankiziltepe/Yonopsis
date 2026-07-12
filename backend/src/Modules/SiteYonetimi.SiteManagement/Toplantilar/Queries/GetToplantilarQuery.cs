using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Toplantilar.DTOs;

namespace SiteYonetimi.SiteManagement.Toplantilar.Queries;

public record GetToplantilarQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, ToplamtiDurum? Durum = null)
    : IRequest<Result<PaginatedResult<ToplamtiDto>>>;

public class GetToplantilarQueryHandler : IRequestHandler<GetToplantilarQuery, Result<PaginatedResult<ToplamtiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetToplantilarQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<ToplamtiDto>>> Handle(GetToplantilarQuery request, CancellationToken ct)
    {
        var q = _db.Toplantilar.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s)); }
        if (request.Durum.HasValue) q = q.Where(x => x.Durum == request.Durum.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.ToplamtiTarihi).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new ToplamtiDto(x.Id, x.Baslik, x.Aciklama, x.Gundem, x.ToplamtiTarihi, x.Konum, x.Durum, x.Katilimcilar, x.Kararlar, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<ToplamtiDto>>.Success(new PaginatedResult<ToplamtiDto>(items, total, request.Page, request.PageSize));
    }
}
