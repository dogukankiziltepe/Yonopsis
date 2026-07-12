using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.DaireSayaclar.DTOs;

namespace SiteYonetimi.SiteManagement.DaireSayaclar.Queries;

public record GetDaireSayaclarQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, Guid? AnaSayacId = null, SayacTipi? Tip = null)
    : IRequest<Result<PaginatedResult<DaireSayacDto>>>;

public class GetDaireSayaclarQueryHandler : IRequestHandler<GetDaireSayaclarQuery, Result<PaginatedResult<DaireSayacDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetDaireSayaclarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<DaireSayacDto>>> Handle(GetDaireSayaclarQuery request, CancellationToken ct)
    {
        var q = _db.DaireSayaclar
            .Include(x => x.Unit).Include(x => x.AnaSayac)
            .Where(x => x.SiteId == request.SiteId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => (x.SeriNo != null && x.SeriNo.ToLower().Contains(s))
                           || (x.Unit != null && x.Unit.DoorNumber.ToLower().Contains(s)));
        }
        if (request.AnaSayacId.HasValue) q = q.Where(x => x.AnaSayacId == request.AnaSayacId.Value);
        if (request.Tip.HasValue) q = q.Where(x => x.Tip == request.Tip.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Unit != null ? x.Unit.DoorNumber : "").Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new DaireSayacDto(x.Id, x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null, x.AnaSayacId, x.AnaSayac != null ? x.AnaSayac.Ad : null, x.Tip, x.SeriNo, x.Marka, x.TakimTarihi, x.Aciklama, x.IsActive, x.CreatedAt))
            .ToListAsync(ct);
        return Result<PaginatedResult<DaireSayacDto>>.Success(new PaginatedResult<DaireSayacDto>(items, total, request.Page, request.PageSize));
    }
}
