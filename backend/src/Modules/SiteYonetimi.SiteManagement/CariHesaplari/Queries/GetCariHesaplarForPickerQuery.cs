using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.CariHesaplari.Queries;

public record CariHesapPickerDto(Guid Id, string HesapKodu, string HesapAdi, CariTuru? CariTuru);

public record GetCariHesaplarForPickerQuery(Guid SiteId, string? Search = null)
    : IRequest<Result<List<CariHesapPickerDto>>>;

/// <summary>
/// Ödeme Makbuzu / Gelir Tahsilat Makbuzu formlarında Cari Hesap seçimi için
/// Muhasebe modülünün HesapPlani tablosunu hafifçe sorgular. Muhasebe modülüne
/// proje referansı eklenmez — HesapPlani zaten SharedTenantDbContext üzerinden
/// erişilebilir (GetUnitFullDetailQueryHandler'daki desenle tutarlı).
/// </summary>
public class GetCariHesaplarForPickerQueryHandler : IRequestHandler<GetCariHesaplarForPickerQuery, Result<List<CariHesapPickerDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetCariHesaplarForPickerQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<CariHesapPickerDto>>> Handle(GetCariHesaplarForPickerQuery request, CancellationToken cancellationToken)
    {
        var query = _db.HesapPlani
            .Where(h => h.SiteId == request.SiteId && h.CariTuru != null && h.AktifMi);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(h => h.HesapAdi.ToLower().Contains(term) || h.HesapKodu.ToLower().Contains(term));
        }

        var list = await query
            .OrderBy(h => h.HesapAdi)
            .Take(50)
            .Select(h => new CariHesapPickerDto(h.Id, h.HesapKodu, h.HesapAdi, h.CariTuru))
            .ToListAsync(cancellationToken);

        return Result<List<CariHesapPickerDto>>.Success(list);
    }
}
