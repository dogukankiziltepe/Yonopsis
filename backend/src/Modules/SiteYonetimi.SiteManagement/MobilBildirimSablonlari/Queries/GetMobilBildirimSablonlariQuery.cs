using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.MobilBildirimSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.MobilBildirimSablonlari.Queries;

public record GetMobilBildirimSablonlariQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<MobilBildirimSablonuDto>>>;

public class GetMobilBildirimSablonlariQueryHandler : IRequestHandler<GetMobilBildirimSablonlariQuery, Result<PaginatedResult<MobilBildirimSablonuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetMobilBildirimSablonlariQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<MobilBildirimSablonuDto>>> Handle(GetMobilBildirimSablonlariQuery request, CancellationToken ct)
    {
        var q = _db.MobilBildirimSablonlari.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Ad.ToLower().Contains(s) || x.Baslik.ToLower().Contains(s)); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new MobilBildirimSablonuDto(x.Id, x.Ad, x.Baslik, x.Icerik, x.Kategori, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<MobilBildirimSablonuDto>>.Success(new PaginatedResult<MobilBildirimSablonuDto>(items, total, request.Page, request.PageSize));
    }
}
