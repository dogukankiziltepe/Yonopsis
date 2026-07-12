using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OrtakAlanlar.DTOs;

namespace SiteYonetimi.SiteManagement.OrtakAlanlar.Queries;

public record GetOrtakAlanlarQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<OrtakAlanDto>>>;

public class GetOrtakAlanlarQueryHandler : IRequestHandler<GetOrtakAlanlarQuery, Result<PaginatedResult<OrtakAlanDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetOrtakAlanlarQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<OrtakAlanDto>>> Handle(GetOrtakAlanlarQuery request, CancellationToken ct)
    {
        var q = _db.OrtakAlanlar.Where(x => x.SiteId == request.SiteId);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Ad.ToLower().Contains(s)); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new OrtakAlanDto(x.Id, x.Ad, x.Aciklama, x.Konum, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<OrtakAlanDto>>.Success(new PaginatedResult<OrtakAlanDto>(items, total, request.Page, request.PageSize));
    }
}

public record GetAllOrtakAlanlarQuery(Guid SiteId) : IRequest<Result<List<OrtakAlanDto>>>;
public class GetAllOrtakAlanlarQueryHandler : IRequestHandler<GetAllOrtakAlanlarQuery, Result<List<OrtakAlanDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAllOrtakAlanlarQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<List<OrtakAlanDto>>> Handle(GetAllOrtakAlanlarQuery request, CancellationToken ct)
    {
        var items = await _db.OrtakAlanlar.Where(x => x.SiteId == request.SiteId && x.IsActive)
            .OrderBy(x => x.Ad).Select(x => new OrtakAlanDto(x.Id, x.Ad, x.Aciklama, x.Konum, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<List<OrtakAlanDto>>.Success(items);
    }
}
