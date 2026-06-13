using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Raporlar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Raporlar.Queries;

/// <summary>Mizan: hesap bazlı borç/alacak toplamları ve bakiyeler (onaylı fişler).</summary>
public record GetMizanQuery(Guid SiteId, DateOnly? From = null, DateOnly? To = null)
    : IRequest<Result<MizanDto>>;

public class GetMizanQueryHandler : IRequestHandler<GetMizanQuery, Result<MizanDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetMizanQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<MizanDto>> Handle(GetMizanQuery request, CancellationToken cancellationToken)
    {
        var query =
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            join h in _db.HesapPlani on d.HesapId equals h.Id
            where d.SiteId == request.SiteId && f.Durum == FisDurumu.Onaylandi
            select new { d, f, h };

        if (request.From is not null) query = query.Where(x => x.f.FisTarihi >= request.From);
        if (request.To is not null) query = query.Where(x => x.f.FisTarihi <= request.To);

        var gruplar = await query
            .GroupBy(x => new { x.d.HesapId, x.d.HesapKodu, x.h.HesapAdi })
            .Select(g => new
            {
                g.Key.HesapId,
                g.Key.HesapKodu,
                g.Key.HesapAdi,
                BorcToplam = g.Sum(x => x.d.BorcTutar),
                AlacakToplam = g.Sum(x => x.d.AlacakTutar)
            })
            .ToListAsync(cancellationToken);

        var satirlar = gruplar
            .OrderBy(g => g.HesapKodu, StringComparer.Ordinal)
            .Select(g =>
            {
                var net = g.BorcToplam - g.AlacakToplam;
                return new MizanSatirDto(
                    g.HesapId, g.HesapKodu, g.HesapAdi, g.BorcToplam, g.AlacakToplam,
                    net > 0 ? net : 0, net < 0 ? -net : 0);
            })
            .ToList();

        var dto = new MizanDto(
            satirlar.Sum(s => s.BorcToplam),
            satirlar.Sum(s => s.AlacakToplam),
            satirlar.Sum(s => s.BorcBakiye),
            satirlar.Sum(s => s.AlacakBakiye),
            satirlar);

        return Result<MizanDto>.Success(dto);
    }
}
