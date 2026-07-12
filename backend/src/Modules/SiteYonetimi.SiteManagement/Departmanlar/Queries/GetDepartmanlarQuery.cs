using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Departmanlar.DTOs;

namespace SiteYonetimi.SiteManagement.Departmanlar.Queries;

public record GetDepartmanlarQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<DepartmanDto>>>;

public class GetDepartmanlarQueryHandler : IRequestHandler<GetDepartmanlarQuery, Result<PaginatedResult<DepartmanDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetDepartmanlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<DepartmanDto>>> Handle(GetDepartmanlarQuery request, CancellationToken ct)
    {
        var q = _db.Departmanlar.Where(x => x.SiteId == request.SiteId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Ad.ToLower().Contains(s));
        }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new DepartmanDto(x.Id, x.Ad, x.Aciklama, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<DepartmanDto>>.Success(new PaginatedResult<DepartmanDto>(items, total, request.Page, request.PageSize));
    }
}

public record GetAllDepartmanlarQuery(Guid SiteId) : IRequest<Result<List<DepartmanDto>>>;

public class GetAllDepartmanlarQueryHandler : IRequestHandler<GetAllDepartmanlarQuery, Result<List<DepartmanDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAllDepartmanlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<DepartmanDto>>> Handle(GetAllDepartmanlarQuery request, CancellationToken ct)
    {
        var items = await _db.Departmanlar.Where(x => x.SiteId == request.SiteId && x.IsActive)
            .OrderBy(x => x.Ad).Select(x => new DepartmanDto(x.Id, x.Ad, x.Aciklama, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<List<DepartmanDto>>.Success(items);
    }
}
