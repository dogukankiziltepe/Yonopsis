using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Fisler.Queries;

public record GetFisDetayQuery(Guid SiteId, Guid Id) : IRequest<Result<FisDetayDto>>;

public class GetFisDetayQueryHandler : IRequestHandler<GetFisDetayQuery, Result<FisDetayDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetFisDetayQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<FisDetayDto>> Handle(GetFisDetayQuery request, CancellationToken cancellationToken)
    {
        var fis = await _db.MuhasebeFisleri
            .Include(f => f.Detaylar)
            .FirstOrDefaultAsync(f => f.SiteId == request.SiteId && f.Id == request.Id, cancellationToken);
        if (fis is null)
            return Result<FisDetayDto>.Failure("Fiş bulunamadı.");

        var hesapIdler = fis.Detaylar.Select(d => d.HesapId).Distinct().ToList();
        var hesapAdlari = await _db.HesapPlani
            .Where(h => h.SiteId == request.SiteId && hesapIdler.Contains(h.Id))
            .ToDictionaryAsync(h => h.Id, h => h.HesapAdi, cancellationToken);

        var satirlar = fis.Detaylar
            .OrderBy(d => d.SiraNo)
            .Select(d => new FisDetaySatirViewDto(
                d.Id, d.SiraNo, d.HesapId, d.HesapKodu,
                hesapAdlari.GetValueOrDefault(d.HesapId, string.Empty),
                d.BorcTutar, d.AlacakTutar, d.Aciklama, d.BelgeNo))
            .ToList();

        var dto = new FisDetayDto(
            fis.Id, fis.DonemId, fis.FisNo, fis.YevmiyeNo, fis.FisTarihi, fis.FisTuru,
            fis.Aciklama, fis.Durum, fis.ToplamBorc, fis.ToplamAlacak, fis.SatirSayisi, satirlar);

        return Result<FisDetayDto>.Success(dto);
    }
}
