using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TalepTipleri.DTOs;

namespace SiteYonetimi.SiteManagement.TalepTipleri.Queries;

public record GetTalepTipleriQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<TalepTipiDto>>>;

public class GetTalepTipleriQueryHandler : IRequestHandler<GetTalepTipleriQuery, Result<PaginatedResult<TalepTipiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetTalepTipleriQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<TalepTipiDto>>> Handle(GetTalepTipleriQuery request, CancellationToken ct)
    {
        var q = _db.TalepTipleri.Where(x => x.SiteId == request.SiteId);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Ad.ToLower().Contains(s)); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new TalepTipiDto(x.Id, x.Ad, x.Aciklama, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<TalepTipiDto>>.Success(new PaginatedResult<TalepTipiDto>(items, total, request.Page, request.PageSize));
    }
}

public record GetAllTalepTipleriQuery(Guid SiteId) : IRequest<Result<List<TalepTipiDto>>>;
public class GetAllTalepTipleriQueryHandler : IRequestHandler<GetAllTalepTipleriQuery, Result<List<TalepTipiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAllTalepTipleriQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<List<TalepTipiDto>>> Handle(GetAllTalepTipleriQuery request, CancellationToken ct)
    {
        var items = await _db.TalepTipleri.Where(x => x.SiteId == request.SiteId && x.IsActive)
            .OrderBy(x => x.Ad).Select(x => new TalepTipiDto(x.Id, x.Ad, x.Aciklama, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<List<TalepTipiDto>>.Success(items);
    }
}
