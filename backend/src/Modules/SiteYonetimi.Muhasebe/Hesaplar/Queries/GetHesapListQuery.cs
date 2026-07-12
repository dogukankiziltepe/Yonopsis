using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Hesaplar.Queries;

/// <summary>Hesap planını düz liste olarak filtreleyerek döner.</summary>
public record GetHesapListQuery(
    Guid SiteId,
    CariTuru? CariTuru = null,
    bool? FisKesilebilir = null,
    bool? Aktif = null,
    string? Search = null) : IRequest<Result<List<HesapListItemDto>>>;

public class GetHesapListQueryHandler
    : IRequestHandler<GetHesapListQuery, Result<List<HesapListItemDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetHesapListQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<HesapListItemDto>>> Handle(GetHesapListQuery request, CancellationToken cancellationToken)
    {
        var query = _db.HesapPlani.Where(h => h.SiteId == request.SiteId);

        if (request.CariTuru is not null)
            query = query.Where(h => h.CariTuru == request.CariTuru);
        if (request.FisKesilebilir is not null)
            query = query.Where(h => h.FisKesilebilirMi == request.FisKesilebilir);
        if (request.Aktif is not null)
            query = query.Where(h => h.AktifMi == request.Aktif);
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
                h.CariTuru, h.PersonId, h.VergiDairesi, h.VergiNumarasi))
            .ToListAsync(cancellationToken);

        return Result<List<HesapListItemDto>>.Success(liste);
    }
}
