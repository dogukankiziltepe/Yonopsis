using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TelefonRehberi.DTOs;

namespace SiteYonetimi.SiteManagement.TelefonRehberi.Queries;

public record GetTelefonRehberiQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null)
    : IRequest<Result<PaginatedResult<TelefonRehberiDto>>>;

public class GetTelefonRehberiQueryHandler : IRequestHandler<GetTelefonRehberiQuery, Result<PaginatedResult<TelefonRehberiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetTelefonRehberiQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<TelefonRehberiDto>>> Handle(GetTelefonRehberiQuery request, CancellationToken ct)
    {
        var q = _db.TelefonRehberi.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Ad.ToLower().Contains(s) || (x.Departman != null && x.Departman.ToLower().Contains(s))); }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Ad).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new TelefonRehberiDto(x.Id, x.Ad, x.Unvan, x.Telefon, x.Dahili, x.Email, x.Departman, x.Aciklama, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<TelefonRehberiDto>>.Success(new PaginatedResult<TelefonRehberiDto>(items, total, request.Page, request.PageSize));
    }
}
