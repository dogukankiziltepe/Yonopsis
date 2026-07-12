using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BirimFiyatlar.DTOs;

namespace SiteYonetimi.SiteManagement.BirimFiyatlar.Queries;

public record GetBirimFiyatlarQuery(Guid SiteId, SayacTipi? Tip = null) : IRequest<Result<List<BirimFiyatDto>>>;

public class GetBirimFiyatlarQueryHandler : IRequestHandler<GetBirimFiyatlarQuery, Result<List<BirimFiyatDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetBirimFiyatlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<BirimFiyatDto>>> Handle(GetBirimFiyatlarQuery request, CancellationToken ct)
    {
        var q = _db.BirimFiyatlar.Where(x => x.SiteId == request.SiteId && !x.IsDeleted);
        if (request.Tip.HasValue) q = q.Where(x => x.Tip == request.Tip.Value);
        var items = await q.OrderByDescending(x => x.BaslangicTarihi)
            .Select(x => new BirimFiyatDto(x.Id, x.Tip, x.Fiyat, x.Birim, x.BaslangicTarihi, x.BitisTarihi, x.Aciklama, x.CreatedAt))
            .ToListAsync(ct);
        return Result<List<BirimFiyatDto>>.Success(items);
    }
}
