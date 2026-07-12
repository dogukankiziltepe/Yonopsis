using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TahsilatMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.TahsilatMakbuzlari.Queries;

public record GetTahsilatMakbuzlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<TahsilatMakbuzuDto>>>;

public class GetTahsilatMakbuzlariQueryHandler : IRequestHandler<GetTahsilatMakbuzlariQuery, Result<PaginatedResult<TahsilatMakbuzuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetTahsilatMakbuzlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<TahsilatMakbuzuDto>>> Handle(GetTahsilatMakbuzlariQuery request, CancellationToken cancellationToken)
    {
        var query = _db.TahsilatMakbuzlari
            .Include(x => x.KasaBanka)
            .Include(x => x.BorcMakbuzu)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BorcluAdi != null && x.BorcluAdi.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new TahsilatMakbuzuDto(
                x.Id, x.EvrakNo, x.IslemTarihi, x.BorcluAdi,
                x.KasaBankaId, x.KasaBanka != null ? x.KasaBanka.Name : null,
                x.BorcMakbuzuId, x.BorcMakbuzu != null ? x.BorcMakbuzu.EvrakNo : null,
                x.OdemeTutari, x.OdemeTipi, x.Aciklama, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<TahsilatMakbuzuDto>>.Success(
            PaginatedResult<TahsilatMakbuzuDto>.Create(items, total, request.Page, request.PageSize));
    }
}
