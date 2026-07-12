using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AjandaEtkinlikleri.DTOs;

namespace SiteYonetimi.SiteManagement.AjandaEtkinlikleri.Queries;

public record GetAjandaEtkinlikleriQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, DateTime? BaslangicFrom = null, DateTime? BaslangicTo = null)
    : IRequest<Result<PaginatedResult<AjandaEtkinlikDto>>>;

public class GetAjandaEtkinlikleriQueryHandler : IRequestHandler<GetAjandaEtkinlikleriQuery, Result<PaginatedResult<AjandaEtkinlikDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAjandaEtkinlikleriQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<AjandaEtkinlikDto>>> Handle(GetAjandaEtkinlikleriQuery request, CancellationToken ct)
    {
        var q = _db.AjandaEtkinlikleri.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s)); }
        if (request.BaslangicFrom.HasValue) q = q.Where(x => x.BaslangicTarihi >= request.BaslangicFrom.Value);
        if (request.BaslangicTo.HasValue) q = q.Where(x => x.BaslangicTarihi <= request.BaslangicTo.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.BaslangicTarihi).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new AjandaEtkinlikDto(x.Id, x.Baslik, x.Aciklama, x.BaslangicTarihi, x.BitisTarihi, x.Konum, x.Renk, x.TumGun, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<AjandaEtkinlikDto>>.Success(new PaginatedResult<AjandaEtkinlikDto>(items, total, request.Page, request.PageSize));
    }
}
