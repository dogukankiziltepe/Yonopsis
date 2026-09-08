using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.DevirBakiyeleri.DTOs;

namespace SiteYonetimi.SiteManagement.DevirBakiyeleri.Queries;

public record GetDevirBakiyeleriQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<DevirBakiyeDto>>>;

public class GetDevirBakiyeleriQueryHandler : IRequestHandler<GetDevirBakiyeleriQuery, Result<PaginatedResult<DevirBakiyeDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetDevirBakiyeleriQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<DevirBakiyeDto>>> Handle(GetDevirBakiyeleriQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DevirBakiyeleri
            .Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BorcluAdiSnapshot != null && x.BorcluAdiSnapshot.ToLower().Contains(term)) ||
                (x.Unit != null && x.Unit.DoorNumber.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Tarih)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new DevirBakiyeDto(
                x.Id, x.EvrakNo, x.Tarih,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.BorcluUserId, x.BorcluAdiSnapshot, x.BorcluRol,
                x.Tutar, x.Aciklama, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<DevirBakiyeDto>>.Success(
            PaginatedResult<DevirBakiyeDto>.Create(items, total, request.Page, request.PageSize));
    }
}
