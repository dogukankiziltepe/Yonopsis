using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Faturalar.DTOs;

namespace SiteYonetimi.SiteManagement.Faturalar.Queries;

public record GetFaturalarQuery(Guid SiteId, FaturaTipi Tip, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<FaturaDto>>>;

public class GetFaturalarQueryHandler : IRequestHandler<GetFaturalarQuery, Result<PaginatedResult<FaturaDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetFaturalarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<FaturaDto>>> Handle(GetFaturalarQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Faturalar
            .Include(x => x.GelirTanimi)
            .Include(x => x.GiderTanimi)
            .Where(x => x.SiteId == request.SiteId && x.Tip == request.Tip);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                x.CariAdi.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.FaturaTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FaturaDto(
                x.Id, x.Tip, x.EvrakNo, x.IslemTarihi, x.FaturaTarihi, x.CariAdi,
                x.GelirTanimiId, x.GelirTanimi != null ? x.GelirTanimi.Name : null,
                x.GiderTanimiId, x.GiderTanimi != null ? x.GiderTanimi.Name : null,
                x.ToplamTutar, x.Aciklama, x.SonOdemeTarihi, x.OdemeDurumu, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<FaturaDto>>.Success(
            PaginatedResult<FaturaDto>.Create(items, total, request.Page, request.PageSize));
    }
}
