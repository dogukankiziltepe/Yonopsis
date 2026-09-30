using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.KasaTransferleri.DTOs;

namespace SiteYonetimi.SiteManagement.KasaTransferleri.Queries;

internal static class KasaTransferProjection
{
    public static IQueryable<KasaTransferDto> ToDto(this IQueryable<KasaTransfer> query) =>
        query.Select(x => new KasaTransferDto(
            x.Id, x.EvrakNo, x.BelgeNo, x.IslemTarihi, x.Tarih,
            x.CikisKasaBankaId, x.CikisKasaBanka != null ? x.CikisKasaBanka.Name : null,
            x.GirisKasaBankaId, x.GirisKasaBanka != null ? x.GirisKasaBanka.Name : null,
            x.Tutar, x.Aciklama, x.CreatedAt));
}

public record GetKasaTransferleriQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<KasaTransferDto>>>;

public class GetKasaTransferleriQueryHandler : IRequestHandler<GetKasaTransferleriQuery, Result<PaginatedResult<KasaTransferDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetKasaTransferleriQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<KasaTransferDto>>> Handle(GetKasaTransferleriQuery request, CancellationToken cancellationToken)
    {
        var query = _db.KasaTransferleri.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BelgeNo != null && x.BelgeNo.ToLower().Contains(term)) ||
                (x.Aciklama != null && x.Aciklama.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToDto()
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<KasaTransferDto>>.Success(
            PaginatedResult<KasaTransferDto>.Create(items, total, request.Page, request.PageSize));
    }
}

public record GetKasaTransferByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<KasaTransferDto>>;

public class GetKasaTransferByIdQueryHandler : IRequestHandler<GetKasaTransferByIdQuery, Result<KasaTransferDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetKasaTransferByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<KasaTransferDto>> Handle(GetKasaTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.KasaTransferleri
            .Where(x => x.Id == request.Id && x.SiteId == request.SiteId)
            .ToDto()
            .FirstOrDefaultAsync(cancellationToken);

        return dto is null
            ? Result<KasaTransferDto>.Failure("Kasa transfer fişi bulunamadı.")
            : Result<KasaTransferDto>.Success(dto);
    }
}
