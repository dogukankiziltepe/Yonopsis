using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Hesaplar.Queries;

/// <summary>Hesap planını hiyerarşik ağaç olarak döner.</summary>
public record GetHesapPlaniTreeQuery(Guid SiteId, bool SadeceAktif = false)
    : IRequest<Result<List<HesapNodeDto>>>;

public class GetHesapPlaniTreeQueryHandler
    : IRequestHandler<GetHesapPlaniTreeQuery, Result<List<HesapNodeDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetHesapPlaniTreeQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<HesapNodeDto>>> Handle(GetHesapPlaniTreeQuery request, CancellationToken cancellationToken)
    {
        var query = _db.HesapPlani.Where(h => h.SiteId == request.SiteId);
        if (request.SadeceAktif)
            query = query.Where(h => h.AktifMi);

        var hepsi = await query
            .OrderBy(h => h.HesapKodu)
            .Select(h => new HesapNodeDto
            {
                Id = h.Id,
                HesapKodu = h.HesapKodu,
                HesapAdi = h.HesapAdi,
                HesapTipi = h.HesapTipi,
                HesapKategorisi = h.HesapKategorisi,
                NormalBakiye = h.NormalBakiye,
                Seviye = h.Seviye,
                FisKesilebilirMi = h.FisKesilebilirMi,
                AktifMi = h.AktifMi,
                CariTuru = h.CariTuru
            })
            .ToListAsync(cancellationToken);

        // ParentId bilgisini ayrı çek (DTO'da tutmuyoruz) ve ağacı kur.
        var parentMap = await query
            .Select(h => new { h.Id, h.ParentId })
            .ToDictionaryAsync(x => x.Id, x => x.ParentId, cancellationToken);

        var nodeMap = hepsi.ToDictionary(n => n.Id);
        var kokler = new List<HesapNodeDto>();

        foreach (var node in hepsi)
        {
            var parentId = parentMap.GetValueOrDefault(node.Id);
            if (parentId is not null && nodeMap.TryGetValue(parentId.Value, out var parent))
                parent.Children.Add(node);
            else
                kokler.Add(node);
        }

        return Result<List<HesapNodeDto>>.Success(kokler);
    }
}
