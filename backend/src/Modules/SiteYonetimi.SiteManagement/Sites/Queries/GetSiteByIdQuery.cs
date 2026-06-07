using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Sites.DTOs;

namespace SiteYonetimi.SiteManagement.Sites.Queries;

public record GetSiteByIdQuery(Guid Id) : IRequest<Result<SiteDetailDto>>;

public class GetSiteByIdQueryHandler : IRequestHandler<GetSiteByIdQuery, Result<SiteDetailDto>>
{
    private readonly MasterDbContext _db;
    private readonly SharedTenantDbContext _sharedDb;

    public GetSiteByIdQueryHandler(MasterDbContext db, SharedTenantDbContext sharedDb)
    {
        _db = db;
        _sharedDb = sharedDb;
    }

    public async Task<Result<SiteDetailDto>> Handle(GetSiteByIdQuery request, CancellationToken cancellationToken)
    {
        var site = await _db.Sites
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (site is null)
            return Result<SiteDetailDto>.Failure("Site bulunamadı.");

        var blockCount = await _sharedDb.Buildings.CountAsync(b => b.SiteId == site.Id, cancellationToken);
        var unitCount = await _sharedDb.Units.CountAsync(u => u.SiteId == site.Id, cancellationToken);

        return Result<SiteDetailDto>.Success(new SiteDetailDto(
            site.Id,
            site.Name,
            blockCount,
            unitCount,
            site.Address,
            site.District,
            site.City,
            site.PostalCode,
            site.Phone,
            site.Email,
            site.TaxOffice,
            site.TaxNumber,
            site.IsActive,
            site.CreatedAt,
            site.UpdatedAt));
    }
}
