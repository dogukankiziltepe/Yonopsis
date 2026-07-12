using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.FotografGalerisi.DTOs;

namespace SiteYonetimi.SiteManagement.FotografGalerisi.Queries;

public record GetFotografGalerisiQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<FotografGalerisiDto>>>;

public class GetFotografGalerisiQueryHandler : IRequestHandler<GetFotografGalerisiQuery, Result<PaginatedResult<FotografGalerisiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetFotografGalerisiQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<FotografGalerisiDto>>> Handle(GetFotografGalerisiQuery request, CancellationToken ct)
    {
        var q = _db.FotografGalerisi.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s)); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Sira).ThenBy(x => x.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new FotografGalerisiDto(x.Id, x.Baslik, x.Aciklama, x.ImageUrl, x.Sira, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<FotografGalerisiDto>>.Success(new PaginatedResult<FotografGalerisiDto>(items, total, request.Page, request.PageSize));
    }
}
