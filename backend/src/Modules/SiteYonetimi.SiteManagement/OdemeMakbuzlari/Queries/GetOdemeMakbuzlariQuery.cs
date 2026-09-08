using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OdemeMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.OdemeMakbuzlari.Queries;

public record GetOdemeMakbuzlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<OdemeMakbuzuDto>>>;

public class GetOdemeMakbuzlariQueryHandler : IRequestHandler<GetOdemeMakbuzlariQuery, Result<PaginatedResult<OdemeMakbuzuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetOdemeMakbuzlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<OdemeMakbuzuDto>>> Handle(GetOdemeMakbuzlariQuery request, CancellationToken cancellationToken)
    {
        var query =
            from o in _db.OdemeMakbuzlari
            where o.SiteId == request.SiteId
            join c in _db.HesapPlani on o.CariHesapId equals c.Id into cariJoin
            from cari in cariJoin.DefaultIfEmpty()
            select new { o, CariHesapAdi = cari != null ? cari.HesapAdi : null };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.o.EvrakNo.ToLower().Contains(term) ||
                (x.CariHesapAdi != null && x.CariHesapAdi.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.o.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new OdemeMakbuzuDto(
                x.o.Id, x.o.EvrakNo, x.o.IslemTarihi, x.o.Tarih,
                x.o.CariHesapId, x.CariHesapAdi,
                x.o.KasaBankaId, x.o.KasaBanka != null ? x.o.KasaBanka.Name : null,
                x.o.GiderTanimiId, x.o.GiderTanimi != null ? x.o.GiderTanimi.Name : null,
                x.o.Tutar, x.o.Aciklama, x.o.DagitimYapilacak, x.o.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<OdemeMakbuzuDto>>.Success(
            PaginatedResult<OdemeMakbuzuDto>.Create(items, total, request.Page, request.PageSize));
    }
}
