using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SmsSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.SmsSablonlari.Queries;

public record GetSmsSablonlariQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<SmsSablonuDto>>>;

public class GetSmsSablonlariQueryHandler : IRequestHandler<GetSmsSablonlariQuery, Result<PaginatedResult<SmsSablonuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetSmsSablonlariQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<SmsSablonuDto>>> Handle(GetSmsSablonlariQuery request, CancellationToken ct)
    {
        var q = _db.SmsSablonlari.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Ad.ToLower().Contains(s)); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new SmsSablonuDto(x.Id, x.Ad, x.Icerik, x.Kategori, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<SmsSablonuDto>>.Success(new PaginatedResult<SmsSablonuDto>(items, total, request.Page, request.PageSize));
    }
}
