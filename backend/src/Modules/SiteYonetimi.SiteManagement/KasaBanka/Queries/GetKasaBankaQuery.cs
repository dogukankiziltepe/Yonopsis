using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.KasaBanka.DTOs;

namespace SiteYonetimi.SiteManagement.KasaBanka.Queries;

public record GetKasaBankaQuery(Guid SiteId) : IRequest<Result<List<KasaBankaDto>>>;

public class GetKasaBankaQueryHandler : IRequestHandler<GetKasaBankaQuery, Result<List<KasaBankaDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetKasaBankaQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<KasaBankaDto>>> Handle(GetKasaBankaQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.KasaBanka
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Tip).ThenBy(x => x.Name)
            .Select(x => new KasaBankaDto(x.Id, x.Name, x.Tip, x.BankaAdi, x.SubeAdi, x.HesapNo, x.IBAN, x.IsActive, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<KasaBankaDto>>.Success(items);
    }
}
