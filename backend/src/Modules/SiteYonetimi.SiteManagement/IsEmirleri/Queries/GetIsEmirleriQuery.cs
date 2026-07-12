using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IsEmirleri.DTOs;

namespace SiteYonetimi.SiteManagement.IsEmirleri.Queries;

public record GetIsEmirleriQuery(
    Guid SiteId, int Page = 1, int PageSize = 20,
    string? Search = null, IsEmriDurum? Durum = null, Guid? DepartmanId = null)
    : IRequest<Result<PaginatedResult<IsEmriDto>>>;

public class GetIsEmirleriQueryHandler : IRequestHandler<GetIsEmirleriQuery, Result<PaginatedResult<IsEmriDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetIsEmirleriQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<IsEmriDto>>> Handle(GetIsEmirleriQuery request, CancellationToken ct)
    {
        var q = _db.IsEmirleri
            .Include(x => x.TalepTipi).Include(x => x.Departman)
            .Include(x => x.OrtakAlan).Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            q = q.Where(x => x.Baslik.ToLower().Contains(s) || (x.AtananKisiAdi != null && x.AtananKisiAdi.ToLower().Contains(s)));
        }
        if (request.Durum.HasValue) q = q.Where(x => x.Durum == request.Durum.Value);
        if (request.DepartmanId.HasValue) q = q.Where(x => x.DepartmanId == request.DepartmanId.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.Oncelik).ThenByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new IsEmriDto(
                x.Id, x.Baslik, x.Aciklama,
                x.TalepTipiId, x.TalepTipi != null ? x.TalepTipi.Ad : null,
                x.DepartmanId, x.Departman != null ? x.Departman.Ad : null,
                x.OrtakAlanId, x.OrtakAlan != null ? x.OrtakAlan.Ad : null,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.Oncelik, x.Durum, x.AtananKisiId, x.AtananKisiAdi,
                x.IslemBaslangic, x.IslemBitis, x.Notlar, x.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedResult<IsEmriDto>>.Success(new PaginatedResult<IsEmriDto>(items, total, request.Page, request.PageSize));
    }
}

public record GetIsEmirleriByDurumQuery(Guid SiteId) : IRequest<Result<Dictionary<IsEmriDurum, List<IsEmriDto>>>>;

public class GetIsEmirleriByDurumQueryHandler : IRequestHandler<GetIsEmirleriByDurumQuery, Result<Dictionary<IsEmriDurum, List<IsEmriDto>>>>
{
    private readonly SharedTenantDbContext _db;
    public GetIsEmirleriByDurumQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Dictionary<IsEmriDurum, List<IsEmriDto>>>> Handle(GetIsEmirleriByDurumQuery request, CancellationToken ct)
    {
        var items = await _db.IsEmirleri
            .Include(x => x.TalepTipi).Include(x => x.Departman).Include(x => x.OrtakAlan).Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId && x.Durum != IsEmriDurum.Iptal)
            .OrderByDescending(x => x.Oncelik).ThenByDescending(x => x.CreatedAt)
            .Select(x => new IsEmriDto(
                x.Id, x.Baslik, x.Aciklama,
                x.TalepTipiId, x.TalepTipi != null ? x.TalepTipi.Ad : null,
                x.DepartmanId, x.Departman != null ? x.Departman.Ad : null,
                x.OrtakAlanId, x.OrtakAlan != null ? x.OrtakAlan.Ad : null,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.Oncelik, x.Durum, x.AtananKisiId, x.AtananKisiAdi,
                x.IslemBaslangic, x.IslemBitis, x.Notlar, x.CreatedAt))
            .ToListAsync(ct);

        var result = new Dictionary<IsEmriDurum, List<IsEmriDto>>
        {
            [IsEmriDurum.YeniTalep] = items.Where(x => x.Durum == IsEmriDurum.YeniTalep).ToList(),
            [IsEmriDurum.Atandi]    = items.Where(x => x.Durum == IsEmriDurum.Atandi).ToList(),
            [IsEmriDurum.Devam]     = items.Where(x => x.Durum == IsEmriDurum.Devam).ToList(),
            [IsEmriDurum.Tamamlandi]= items.Where(x => x.Durum == IsEmriDurum.Tamamlandi).ToList(),
        };
        return Result<Dictionary<IsEmriDurum, List<IsEmriDto>>>.Success(result);
    }
}
