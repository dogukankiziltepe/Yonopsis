using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Queries;

/// <summary>
/// "Muhasebe Fişi Detayları Yönetimi" ekranı: tüm fiş satırlarının düz,
/// filtrelenebilir ve sayfalı listesi. Silinmiş fişlerin satırları hariç tutulur.
/// </summary>
public record GetFisDetaylarQuery(
    Guid SiteId,
    Guid? HesapId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    FisDurumu? Durum = null,
    int Page = 1,
    int PageSize = 50) : IRequest<Result<PaginatedResult<FisDetaylarItemDto>>>;

public class GetFisDetaylarQueryHandler
    : IRequestHandler<GetFisDetaylarQuery, Result<PaginatedResult<FisDetaylarItemDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetFisDetaylarQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResult<FisDetaylarItemDto>>> Handle(GetFisDetaylarQuery request, CancellationToken cancellationToken)
    {
        // Fişlere join: MuhasebeFisleri query filter'ı (!IsDeleted) silinmiş fiş satırlarını eler.
        var query =
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            join h in _db.HesapPlani on d.HesapId equals h.Id
            where d.SiteId == request.SiteId
            select new { d, f, h };

        if (request.HesapId is not null) query = query.Where(x => x.d.HesapId == request.HesapId);
        if (request.From is not null) query = query.Where(x => x.f.FisTarihi >= request.From);
        if (request.To is not null) query = query.Where(x => x.f.FisTarihi <= request.To);
        if (request.Durum is not null) query = query.Where(x => x.f.Durum == request.Durum);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.f.FisTarihi)
            .ThenBy(x => x.f.FisNo)
            .ThenBy(x => x.d.SiraNo)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FisDetaylarItemDto(
                x.d.Id, x.f.Id, x.f.FisNo, x.f.YevmiyeNo, x.f.FisTarihi, x.f.Durum,
                x.d.SiraNo, x.d.HesapId, x.d.HesapKodu, x.h.HesapAdi,
                x.d.BorcTutar, x.d.AlacakTutar, x.d.Aciklama, x.d.BelgeNo))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<FisDetaylarItemDto>>.Success(
            PaginatedResult<FisDetaylarItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}
