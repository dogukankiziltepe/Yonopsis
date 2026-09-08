using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirTahsilatMakbuzlari.Queries;

public record GetGelirTahsilatMakbuzlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<GelirTahsilatMakbuzuDto>>>;

public class GetGelirTahsilatMakbuzlariQueryHandler : IRequestHandler<GetGelirTahsilatMakbuzlariQuery, Result<PaginatedResult<GelirTahsilatMakbuzuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetGelirTahsilatMakbuzlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<GelirTahsilatMakbuzuDto>>> Handle(GetGelirTahsilatMakbuzlariQuery request, CancellationToken cancellationToken)
    {
        var query =
            from g in _db.GelirTahsilatMakbuzlari
            where g.SiteId == request.SiteId
            join c in _db.HesapPlani on g.CariHesapId equals c.Id into cariJoin
            from cari in cariJoin.DefaultIfEmpty()
            select new { g, CariHesapAdi = cari != null ? cari.HesapAdi : null };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.g.EvrakNo.ToLower().Contains(term) ||
                (x.CariHesapAdi != null && x.CariHesapAdi.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.g.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GelirTahsilatMakbuzuDto(
                x.g.Id, x.g.EvrakNo, x.g.IslemTarihi, x.g.Tarih,
                x.g.CariHesapId, x.CariHesapAdi,
                x.g.KasaBankaId, x.g.KasaBanka != null ? x.g.KasaBanka.Name : null,
                x.g.GelirTanimiId, x.g.GelirTanimi != null ? x.g.GelirTanimi.Name : null,
                x.g.Tutar, x.g.Aciklama, x.g.DagitimYapilacak, x.g.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<GelirTahsilatMakbuzuDto>>.Success(
            PaginatedResult<GelirTahsilatMakbuzuDto>.Create(items, total, request.Page, request.PageSize));
    }
}
