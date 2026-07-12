using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OtomatikBildirimler.DTOs;

namespace SiteYonetimi.SiteManagement.OtomatikBildirimler.Queries;

public record GetOtomatikBildirimlerQuery(Guid SiteId) : IRequest<Result<List<OtomatikBildirimDto>>>;

public class GetOtomatikBildirimlerQueryHandler : IRequestHandler<GetOtomatikBildirimlerQuery, Result<List<OtomatikBildirimDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetOtomatikBildirimlerQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<List<OtomatikBildirimDto>>> Handle(GetOtomatikBildirimlerQuery request, CancellationToken ct)
    {
        var items = await _db.OtomatikBildirimler
            .Where(x => x.SiteId == request.SiteId && !x.IsDeleted)
            .Include(x => x.EpostaSablonu).Include(x => x.SmsSablonu).Include(x => x.MobilSablonu)
            .OrderBy(x => x.OlayTipi)
            .Select(x => new OtomatikBildirimDto(x.Id, x.OlayTipi, x.EpostaAktif, x.SmsAktif, x.MobilAktif,
                x.EpostaSablonuId, x.EpostaSablonu != null ? x.EpostaSablonu.Ad : null,
                x.SmsSablonuId, x.SmsSablonu != null ? x.SmsSablonu.Ad : null,
                x.MobilSablonuId, x.MobilSablonu != null ? x.MobilSablonu.Ad : null,
                x.IsActive, x.CreatedAt))
            .ToListAsync(ct);
        return Result<List<OtomatikBildirimDto>>.Success(items);
    }
}
