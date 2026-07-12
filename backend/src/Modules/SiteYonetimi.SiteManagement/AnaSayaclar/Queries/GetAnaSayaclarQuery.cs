using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.AnaSayaclar.DTOs;

namespace SiteYonetimi.SiteManagement.AnaSayaclar.Queries;

public record GetAnaSayaclarQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, SayacTipi? Tip = null)
    : IRequest<Result<PaginatedResult<AnaSayacDto>>>;

public class GetAnaSayaclarQueryHandler : IRequestHandler<GetAnaSayaclarQuery, Result<PaginatedResult<AnaSayacDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnaSayaclarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<AnaSayacDto>>> Handle(GetAnaSayaclarQuery request, CancellationToken ct)
    {
        var q = _db.AnaSayaclar.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Ad.ToLower().Contains(s) || (x.SeriNo != null && x.SeriNo.ToLower().Contains(s)));
        }
        if (request.Tip.HasValue) q = q.Where(x => x.Tip == request.Tip.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new AnaSayacDto(x.Id, x.Ad, x.Tip, x.SeriNo, x.Marka, x.TakimTarihi, x.Aciklama, x.IsActive, x.CreatedAt))
            .ToListAsync(ct);
        return Result<PaginatedResult<AnaSayacDto>>.Success(new PaginatedResult<AnaSayacDto>(items, total, request.Page, request.PageSize));
    }
}

public record GetAllAnaSayaclarQuery(Guid SiteId) : IRequest<Result<List<AnaSayacDto>>>;
public class GetAllAnaSayaclarQueryHandler : IRequestHandler<GetAllAnaSayaclarQuery, Result<List<AnaSayacDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAllAnaSayaclarQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<List<AnaSayacDto>>> Handle(GetAllAnaSayaclarQuery request, CancellationToken ct)
    {
        var items = await _db.AnaSayaclar.Where(x => x.SiteId == request.SiteId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Ad)
            .Select(x => new AnaSayacDto(x.Id, x.Ad, x.Tip, x.SeriNo, x.Marka, x.TakimTarihi, x.Aciklama, x.IsActive, x.CreatedAt))
            .ToListAsync(ct);
        return Result<List<AnaSayacDto>>.Success(items);
    }
}
