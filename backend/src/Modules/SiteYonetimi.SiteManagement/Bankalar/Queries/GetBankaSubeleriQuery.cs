using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Bankalar.DTOs;

namespace SiteYonetimi.SiteManagement.Bankalar.Queries;

/// <summary>Düz şube listesi — Personel Banka Bilgileri picker'ı için.</summary>
public record GetBankaSubeleriQuery(Guid? BankaId = null, string? Search = null) : IRequest<Result<List<BankaSubesiPickerItemDto>>>;

public record BankaSubesiPickerItemDto(Guid Id, string BankaAdi, string SubeAdi, string? SubeKodu);

public class GetBankaSubeleriQueryHandler : IRequestHandler<GetBankaSubeleriQuery, Result<List<BankaSubesiPickerItemDto>>>
{
    private readonly MasterDbContext _db;
    public GetBankaSubeleriQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<BankaSubesiPickerItemDto>>> Handle(GetBankaSubeleriQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BankaSubeleri.Include(x => x.Banka).Where(x => x.IsActive);

        if (request.BankaId is not null)
            query = query.Where(x => x.BankaId == request.BankaId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(x => x.SubeAdi.Contains(s) || x.Banka.Name.Contains(s));
        }

        var liste = await query
            .OrderBy(x => x.Banka.Name).ThenBy(x => x.SubeAdi)
            .Select(x => new BankaSubesiPickerItemDto(x.Id, x.Banka.Name, x.SubeAdi, x.SubeKodu))
            .ToListAsync(cancellationToken);

        return Result<List<BankaSubesiPickerItemDto>>.Success(liste);
    }
}
