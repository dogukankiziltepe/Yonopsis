using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Donemler.DTOs;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Donemler.Queries;

public record GetDonemlerQuery(Guid SiteId) : IRequest<Result<List<DonemDto>>>;

public class GetDonemlerQueryHandler : IRequestHandler<GetDonemlerQuery, Result<List<DonemDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetDonemlerQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<DonemDto>>> Handle(GetDonemlerQuery request, CancellationToken cancellationToken)
    {
        var liste = await _db.MuhasebeDonemler
            .Where(d => d.SiteId == request.SiteId)
            .OrderByDescending(d => d.Yil)
            .Select(d => new DonemDto(d.Id, d.Yil, d.Ad, d.BaslangicTarihi, d.BitisTarihi, d.Durum, d.SonYevmiyeNo))
            .ToListAsync(cancellationToken);

        return Result<List<DonemDto>>.Success(liste);
    }
}
