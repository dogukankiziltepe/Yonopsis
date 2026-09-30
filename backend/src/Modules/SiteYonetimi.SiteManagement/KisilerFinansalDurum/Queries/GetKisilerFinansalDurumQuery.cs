using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.KisilerFinansalDurum.DTOs;

namespace SiteYonetimi.SiteManagement.KisilerFinansalDurum.Queries;

public record GetKisilerFinansalDurumQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<KisiFinansalDurumSatiriDto>>>;

/// <summary>
/// Sadece Owner+Renter rolündeki kişiler için borç/tahsilat/iade/devir toplamlarını
/// agregе eder. N+1'den kaçınmak için kişi sayısı kadar değil, sabit sayıda (4-5)
/// GroupBy sorgusu çalıştırılır, sonuçlar bellek içinde Dictionary ile birleştirilir.
/// </summary>
public class GetKisilerFinansalDurumQueryHandler : IRequestHandler<GetKisilerFinansalDurumQuery, Result<PaginatedResult<KisiFinansalDurumSatiriDto>>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetKisilerFinansalDurumQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<PaginatedResult<KisiFinansalDurumSatiriDto>>> Handle(GetKisilerFinansalDurumQuery request, CancellationToken cancellationToken)
    {
        var siteId = request.SiteId;

        // Kapsam kişileri: PersonUnitHistory (Owner/Renter) + Unit.OwnerUserId/TenantUserId fallback
        var historyIds = await _db.PersonUnitHistories
            .Where(x => x.SiteId == siteId && (x.Role == UserType.Owner || x.Role == UserType.Renter))
            .Select(x => x.PersonUserId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var ownerIds = await _db.Units.Where(u => u.SiteId == siteId && u.OwnerUserId != null)
            .Select(u => u.OwnerUserId!.Value).Distinct().ToListAsync(cancellationToken);
        var tenantIds = await _db.Units.Where(u => u.SiteId == siteId && u.TenantUserId != null)
            .Select(u => u.TenantUserId!.Value).Distinct().ToListAsync(cancellationToken);
        // Virman satırı olan ama artık daireye bağlı olmayan kişiler (örn. eski malik) de listelensin
        var virmanPersonIds = await _db.VirmanSatirlari
            .Where(s => s.SiteId == siteId && s.HesapTuru == VirmanHesapTuru.Kisi && !s.Virman.IsDeleted)
            .Select(s => s.HesapId).Distinct().ToListAsync(cancellationToken);
        var personIds = historyIds.Union(ownerIds).Union(tenantIds).Union(virmanPersonIds).Distinct().ToList();

        if (personIds.Count == 0)
            return Result<PaginatedResult<KisiFinansalDurumSatiriDto>>.Success(
                PaginatedResult<KisiFinansalDurumSatiriDto>.Create(new List<KisiFinansalDurumSatiriDto>(), 0, request.Page, request.PageSize));

        var borcAgg = await _db.BorcMakbuzlari
            .Where(b => b.SiteId == siteId && b.BorcluUserId != null && personIds.Contains(b.BorcluUserId.Value))
            .GroupBy(b => b.BorcluUserId!.Value)
            .Select(g => new { PersonId = g.Key, BorcTutari = g.Sum(x => x.Tutar), Gecikme = g.Sum(x => x.GecikmeTutari), Odenen = g.Sum(x => x.OdenenTutar), Kalan = g.Sum(x => x.Tutar + x.GecikmeTutari - x.OdenenTutar) })
            .ToDictionaryAsync(x => x.PersonId, cancellationToken);

        var iadeAgg = await _db.IadeMakbuzlari
            .Where(x => x.SiteId == siteId && x.BorcluUserId != null && personIds.Contains(x.BorcluUserId.Value))
            .GroupBy(x => x.BorcluUserId!.Value)
            .Select(g => new { PersonId = g.Key, Toplam = g.Sum(x => x.Tutar) })
            .ToDictionaryAsync(x => x.PersonId, cancellationToken);

        var devirAgg = await _db.DevirBakiyeleri
            .Where(x => x.SiteId == siteId && x.BorcluUserId != null && personIds.Contains(x.BorcluUserId.Value))
            .GroupBy(x => x.BorcluUserId!.Value)
            .Select(g => new { PersonId = g.Key, Toplam = g.Sum(x => x.Tutar) })
            .ToDictionaryAsync(x => x.PersonId, cancellationToken);

        var virmanAgg = await _db.VirmanSatirlari
            .Where(s => s.SiteId == siteId && s.HesapTuru == VirmanHesapTuru.Kisi && !s.Virman.IsDeleted && personIds.Contains(s.HesapId))
            .GroupBy(s => s.HesapId)
            .Select(g => new { PersonId = g.Key, Net = g.Sum(x => x.BorcTutari - x.AlacakTutari) })
            .ToDictionaryAsync(x => x.PersonId, cancellationToken);

        var users = await _masterDb.Users
            .Where(u => personIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var satirlar = new List<KisiFinansalDurumSatiriDto>();
        foreach (var personId in personIds)
        {
            var b = borcAgg.GetValueOrDefault(personId);
            var iade = iadeAgg.GetValueOrDefault(personId)?.Toplam ?? 0m;
            var devir = devirAgg.GetValueOrDefault(personId)?.Toplam ?? 0m;
            var adSoyad = users.TryGetValue(personId, out var u) ? $"{u.FirstName} {u.LastName}" : "-";

            var virman = virmanAgg.GetValueOrDefault(personId)?.Net ?? 0m;
            var netto = (b?.Kalan ?? 0m) + devir - iade + virman;
            var borc = Math.Max(0, netto);
            var alacak = Math.Max(0, -netto);

            satirlar.Add(new KisiFinansalDurumSatiriDto(
                personId, adSoyad,
                b?.BorcTutari ?? 0m, b?.Gecikme ?? 0m, iade, b?.Odenen ?? 0m,
                borc, alacak, borc - alacak, borc - alacak >= 0 ? "B" : "A"));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            satirlar = satirlar.Where(x => x.AdSoyad.ToLower().Contains(term)).ToList();
        }

        satirlar = satirlar.OrderByDescending(x => x.Borc).ThenBy(x => x.AdSoyad).ToList();

        var total = satirlar.Count;
        var page = satirlar.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

        return Result<PaginatedResult<KisiFinansalDurumSatiriDto>>.Success(
            PaginatedResult<KisiFinansalDurumSatiriDto>.Create(page, total, request.Page, request.PageSize));
    }
}
