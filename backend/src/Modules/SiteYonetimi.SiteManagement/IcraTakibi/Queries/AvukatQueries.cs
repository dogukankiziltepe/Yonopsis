using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Queries;

public record GetAvukatlarQuery(Guid SiteId, string? Search = null) : IRequest<Result<List<AvukatDto>>>;

public class GetAvukatlarQueryHandler : IRequestHandler<GetAvukatlarQuery, Result<List<AvukatDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAvukatlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AvukatDto>>> Handle(GetAvukatlarQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Avukatlar.Where(x => x.SiteId == request.SiteId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x => x.AdSoyad.ToLower().Contains(term) || (x.BuroAdi != null && x.BuroAdi.ToLower().Contains(term)));
        }
        var items = await query.OrderBy(x => x.AdSoyad)
            .Select(x => new AvukatDto(x.Id, x.AdSoyad, x.BuroAdi, x.Telefon, x.Eposta, x.Adres, x.IsActive))
            .ToListAsync(cancellationToken);
        return Result<List<AvukatDto>>.Success(items);
    }
}

/// <summary>İcra ekranlarındaki avukat select'i için (Avukatlar sayfası yetkisi gerektirmez).</summary>
public record GetAvukatSecimListesiQuery(Guid SiteId) : IRequest<Result<List<AvukatSecimDto>>>;

public class GetAvukatSecimListesiQueryHandler : IRequestHandler<GetAvukatSecimListesiQuery, Result<List<AvukatSecimDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAvukatSecimListesiQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AvukatSecimDto>>> Handle(GetAvukatSecimListesiQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Avukatlar.Where(x => x.SiteId == request.SiteId && x.IsActive)
            .OrderBy(x => x.AdSoyad)
            .Select(x => new AvukatSecimDto(x.Id, x.AdSoyad))
            .ToListAsync(cancellationToken);
        return Result<List<AvukatSecimDto>>.Success(items);
    }
}
