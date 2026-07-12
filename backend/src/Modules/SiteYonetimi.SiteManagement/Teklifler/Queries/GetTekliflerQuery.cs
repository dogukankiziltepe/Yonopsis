using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Teklifler.DTOs;

namespace SiteYonetimi.SiteManagement.Teklifler.Queries;

public record GetTekliflerQuery(Guid SiteId, int Page = 1, int PageSize = 50, string? Search = null, TeklifDurum? Durum = null)
    : IRequest<Result<PaginatedResult<TeklifDto>>>;

public class GetTekliflerQueryHandler : IRequestHandler<GetTekliflerQuery, Result<PaginatedResult<TeklifDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetTekliflerQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<PaginatedResult<TeklifDto>>> Handle(GetTekliflerQuery request, CancellationToken ct)
    {
        var q = _db.Teklifler.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.ToLower(); q = q.Where(x => x.Baslik.ToLower().Contains(s) || (x.TedarikciAdi != null && x.TedarikciAdi.ToLower().Contains(s))); }
        if (request.Durum.HasValue) q = q.Where(x => x.Durum == request.Durum.Value);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.TeklifTarihi).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new TeklifDto(x.Id, x.Baslik, x.Aciklama, x.TedarikciAdi, x.Tutar, x.TeklifTarihi, x.GecerlilikTarihi, x.Durum, x.Notlar, x.IsActive, x.CreatedAt)).ToListAsync(ct);
        return Result<PaginatedResult<TeklifDto>>.Success(new PaginatedResult<TeklifDto>(items, total, request.Page, request.PageSize));
    }
}
