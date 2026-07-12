using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.EpostaSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.EpostaSablonlari.Queries;

public record GetEpostaSablonlariQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<EpostaSablonuDto>>>;

public class GetEpostaSablonlariQueryHandler : IRequestHandler<GetEpostaSablonlariQuery, Result<PaginatedResult<EpostaSablonuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetEpostaSablonlariQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<EpostaSablonuDto>>> Handle(GetEpostaSablonlariQuery request, CancellationToken ct)
    {
        var q = _db.EpostaSablonlari.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Ad.ToLower().Contains(s) || x.Konu.ToLower().Contains(s));
        }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new EpostaSablonuDto(x.Id, x.Ad, x.Konu, x.IcerikHtml, x.IcerikText, x.Kategori, x.IsActive, x.CreatedAt))
            .ToListAsync(ct);
        return Result<PaginatedResult<EpostaSablonuDto>>.Success(new PaginatedResult<EpostaSablonuDto>(items, total, request.Page, request.PageSize));
    }
}
