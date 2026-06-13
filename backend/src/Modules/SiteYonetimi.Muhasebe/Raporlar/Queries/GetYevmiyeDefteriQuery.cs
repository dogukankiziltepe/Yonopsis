using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Raporlar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Raporlar.Queries;

/// <summary>Yevmiye defteri: tarih aralığındaki onaylı tüm hareketler, sıralı.</summary>
public record GetYevmiyeDefteriQuery(Guid SiteId, DateOnly? From = null, DateOnly? To = null)
    : IRequest<Result<YevmiyeDefteriDto>>;

public class GetYevmiyeDefteriQueryHandler : IRequestHandler<GetYevmiyeDefteriQuery, Result<YevmiyeDefteriDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetYevmiyeDefteriQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<YevmiyeDefteriDto>> Handle(GetYevmiyeDefteriQuery request, CancellationToken cancellationToken)
    {
        var query =
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            join h in _db.HesapPlani on d.HesapId equals h.Id
            where d.SiteId == request.SiteId && f.Durum == FisDurumu.Onaylandi
            select new { d, f, h };

        if (request.From is not null) query = query.Where(x => x.f.FisTarihi >= request.From);
        if (request.To is not null) query = query.Where(x => x.f.FisTarihi <= request.To);

        var satirlar = await query
            .OrderBy(x => x.f.FisTarihi).ThenBy(x => x.f.YevmiyeNo).ThenBy(x => x.d.SiraNo)
            .Select(x => new YevmiyeSatirDto(
                x.f.YevmiyeNo, x.f.FisNo, x.f.FisTarihi, x.f.FisTuru,
                x.d.HesapKodu, x.h.HesapAdi, x.d.Aciklama, x.d.BorcTutar, x.d.AlacakTutar))
            .ToListAsync(cancellationToken);

        var dto = new YevmiyeDefteriDto(
            satirlar.Sum(s => s.BorcTutar), satirlar.Sum(s => s.AlacakTutar), satirlar);

        return Result<YevmiyeDefteriDto>.Success(dto);
    }
}
