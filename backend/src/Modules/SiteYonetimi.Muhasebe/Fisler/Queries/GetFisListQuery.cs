using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Queries;

public record GetFisListQuery(
    Guid SiteId,
    DateOnly? From = null,
    DateOnly? To = null,
    FisTuru? Tur = null,
    FisDurumu? Durum = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedResult<FisListItemDto>>>;

public class GetFisListQueryHandler : IRequestHandler<GetFisListQuery, Result<PaginatedResult<FisListItemDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetFisListQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResult<FisListItemDto>>> Handle(GetFisListQuery request, CancellationToken cancellationToken)
    {
        var query = _db.MuhasebeFisleri.Where(f => f.SiteId == request.SiteId);

        if (request.From is not null) query = query.Where(f => f.FisTarihi >= request.From);
        if (request.To is not null) query = query.Where(f => f.FisTarihi <= request.To);
        if (request.Tur is not null) query = query.Where(f => f.FisTuru == request.Tur);
        if (request.Durum is not null) query = query.Where(f => f.Durum == request.Durum);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(f => f.FisNo.Contains(s) || f.Aciklama.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.FisTarihi)
            .ThenByDescending(f => f.YevmiyeNo)
            .ThenByDescending(f => f.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FisListItemDto(
                f.Id, f.FisNo, f.YevmiyeNo, f.FisTarihi, f.FisTuru, f.Aciklama,
                f.Durum, f.ToplamBorc, f.ToplamAlacak, f.SatirSayisi))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<FisListItemDto>>.Success(
            PaginatedResult<FisListItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}
