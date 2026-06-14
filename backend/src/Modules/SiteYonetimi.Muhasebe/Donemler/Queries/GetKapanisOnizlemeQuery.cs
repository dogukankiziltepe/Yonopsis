using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Muhasebe.Donemler.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.Queries;

/// <summary>Dönem sonu kapanış önizlemesi: hesap bakiyeleri + gelir/gider/net sonuç.</summary>
public record GetKapanisOnizlemeQuery(Guid SiteId, Guid DonemId) : IRequest<Result<KapanisOnizlemeDto>>;

public class GetKapanisOnizlemeQueryHandler : IRequestHandler<GetKapanisOnizlemeQuery, Result<KapanisOnizlemeDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly IDonemSonuService _donemSonuService;

    public GetKapanisOnizlemeQueryHandler(SharedTenantDbContext db, IDonemSonuService donemSonuService)
    {
        _db = db;
        _donemSonuService = donemSonuService;
    }

    public async Task<Result<KapanisOnizlemeDto>> Handle(GetKapanisOnizlemeQuery request, CancellationToken cancellationToken)
    {
        var donem = await _db.MuhasebeDonemler
            .FirstOrDefaultAsync(d => d.SiteId == request.SiteId && d.Id == request.DonemId, cancellationToken);
        if (donem is null)
            return Result<KapanisOnizlemeDto>.Failure("Dönem bulunamadı.");

        var bakiyeler = await _donemSonuService.HesapBakiyeleriAsync(request.SiteId, request.DonemId, cancellationToken);

        // Gelir: Alacak yönlü (net < 0). Gider/Maliyet: Borç yönlü (net > 0).
        var toplamGelir = bakiyeler.Where(b => b.Kategori == HesapKategorisi.Gelir).Sum(b => -b.Net);
        var toplamGider = bakiyeler
            .Where(b => b.Kategori is HesapKategorisi.Gider or HesapKategorisi.Maliyet)
            .Sum(b => b.Net);

        var dengeli = bakiyeler.Sum(b => b.Net) == 0;

        var dto = new KapanisOnizlemeDto(
            donem.Id, donem.Yil, toplamGelir, toplamGider, toplamGelir - toplamGider, dengeli, bakiyeler);

        return Result<KapanisOnizlemeDto>.Success(dto);
    }
}
