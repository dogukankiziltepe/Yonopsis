using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.BankaHareketleri.DTOs;

namespace SiteYonetimi.SiteManagement.BankaHareketleri.Queries;

public record GetBankaHareketleriQuery(Guid SiteId, Guid? KasaBankaId = null, int Page = 1, int PageSize = 20)
    : IRequest<Result<PaginatedResult<BankaHareketiDto>>>;

public class GetBankaHareketleriQueryHandler : IRequestHandler<GetBankaHareketleriQuery, Result<PaginatedResult<BankaHareketiDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetBankaHareketleriQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<BankaHareketiDto>>> Handle(GetBankaHareketleriQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BankaHareketleri
            .Include(x => x.KasaBanka)
            .Where(x => x.SiteId == request.SiteId);

        if (request.KasaBankaId.HasValue)
            query = query.Where(x => x.KasaBankaId == request.KasaBankaId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Tarih)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new BankaHareketiDto(
                x.Id, x.KasaBankaId, x.KasaBanka.Name,
                x.Tarih, x.Aciklama, x.ReferansNo,
                x.Tutar, x.Durum, x.EslestirmeId, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<BankaHareketiDto>>.Success(
            PaginatedResult<BankaHareketiDto>.Create(items, total, request.Page, request.PageSize));
    }
}
