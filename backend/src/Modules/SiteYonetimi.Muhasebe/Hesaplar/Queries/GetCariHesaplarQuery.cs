using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Hesaplar.Queries;

/// <summary>Cari hesapları (HesapTipi=Cari) opsiyonel CariTuru filtresiyle döner.</summary>
public record GetCariHesaplarQuery(Guid SiteId, CariTuru? CariTuru = null, string? Search = null)
    : IRequest<Result<List<HesapListItemDto>>>;

public class GetCariHesaplarQueryHandler
    : IRequestHandler<GetCariHesaplarQuery, Result<List<HesapListItemDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetCariHesaplarQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<HesapListItemDto>>> Handle(GetCariHesaplarQuery request, CancellationToken cancellationToken)
    {
        var query = _db.HesapPlani
            .Where(h => h.SiteId == request.SiteId && h.HesapTipi == HesapTipi.Cari);

        if (request.CariTuru is not null)
            query = query.Where(h => h.CariTuru == request.CariTuru);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(h => h.HesapKodu.Contains(s) || h.HesapAdi.Contains(s));
        }

        var liste = await query
            .OrderBy(h => h.HesapKodu)
            .Select(h => new HesapListItemDto(
                h.Id, h.HesapKodu, h.HesapAdi, h.HesapTipi, h.HesapKategorisi,
                h.NormalBakiye, h.Seviye, h.ParentId, h.FisKesilebilirMi, h.AktifMi,
                h.CariTuru, h.PersonId))
            .ToListAsync(cancellationToken);

        return Result<List<HesapListItemDto>>.Success(liste);
    }
}
