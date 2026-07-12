using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.KayipEsya.DTOs;

namespace SiteYonetimi.SiteManagement.KayipEsya.Queries;

public record GetKayipEsyalarQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null, KayipEsyaDurum? Durum = null)
    : IRequest<Result<PaginatedResult<KayipEsyaDto>>>;

public class GetKayipEsyalarQueryHandler : IRequestHandler<GetKayipEsyalarQuery, Result<PaginatedResult<KayipEsyaDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetKayipEsyalarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<KayipEsyaDto>>> Handle(GetKayipEsyalarQuery request, CancellationToken ct)
    {
        var q = _db.KayipEsyalar.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.EsyaAdi.ToLower().Contains(s) || (x.SahipAdi != null && x.SahipAdi.ToLower().Contains(s)));
        }

        if (request.Durum.HasValue)
            q = q.Where(x => x.Durum == request.Durum.Value);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.BulunanTarih)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new KayipEsyaDto(
                x.Id, x.EsyaAdi, x.Aciklama, x.BulunanYer, x.BulunanTarih,
                x.SahipAdi, x.SahipIletisim, x.Durum, x.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedResult<KayipEsyaDto>>.Success(
            new PaginatedResult<KayipEsyaDto>(items, total, request.Page, request.PageSize));
    }
}
