using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Anketler.DTOs;

namespace SiteYonetimi.SiteManagement.Anketler.Queries;

public record GetAnketlerQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, AnketDurum? Durum = null)
    : IRequest<Result<PaginatedResult<AnketDto>>>;

public class GetAnketlerQueryHandler : IRequestHandler<GetAnketlerQuery, Result<PaginatedResult<AnketDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnketlerQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<AnketDto>>> Handle(GetAnketlerQuery request, CancellationToken ct)
    {
        var q = _db.Anketler.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s)); }
        if (request.Durum.HasValue) q = q.Where(x => x.Durum == request.Durum.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new AnketDto(x.Id, x.Baslik, x.Aciklama, x.BaslangicTarihi, x.BitisTarihi, x.Durum, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<AnketDto>>.Success(new PaginatedResult<AnketDto>(items, total, request.Page, request.PageSize));
    }
}
