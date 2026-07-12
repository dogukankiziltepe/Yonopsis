using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SayacOkumalar.DTOs;

namespace SiteYonetimi.SiteManagement.SayacOkumalar.Queries;

public record GetSayacOkumalarQuery(Guid SiteId, int Page = 1, int PageSize = 30, Guid? AnaSayacId = null, Guid? DaireSayacId = null)
    : IRequest<Result<PaginatedResult<SayacOkumaDto>>>;

public class GetSayacOkumalarQueryHandler : IRequestHandler<GetSayacOkumalarQuery, Result<PaginatedResult<SayacOkumaDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetSayacOkumalarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<SayacOkumaDto>>> Handle(GetSayacOkumalarQuery request, CancellationToken ct)
    {
        var q = _db.SayacOkumalar
            .Include(x => x.AnaSayac).Include(x => x.DaireSayac).ThenInclude(d => d != null ? d.Unit : null)
            .Where(x => x.SiteId == request.SiteId && !x.IsDeleted);

        if (request.AnaSayacId.HasValue) q = q.Where(x => x.AnaSayacId == request.AnaSayacId.Value);
        if (request.DaireSayacId.HasValue) q = q.Where(x => x.DaireSayacId == request.DaireSayacId.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.OkumaTarihi).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new SayacOkumaDto(
                x.Id, x.AnaSayacId, x.AnaSayac != null ? x.AnaSayac.Ad : null,
                x.DaireSayacId, x.DaireSayac != null && x.DaireSayac.Unit != null ? x.DaireSayac.Unit.DoorNumber : null,
                x.OkumaTarihi, x.OncekiEndeks, x.SonEndeks, x.Tuketim, x.Aciklama, x.CreatedAt))
            .ToListAsync(ct);
        return Result<PaginatedResult<SayacOkumaDto>>.Success(new PaginatedResult<SayacOkumaDto>(items, total, request.Page, request.PageSize));
    }
}
