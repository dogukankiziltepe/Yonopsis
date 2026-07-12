using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.BorcMakbuzlari.Queries;

public record GetBorcMakbuzlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<BorcMakbuzuDto>>>;

public class GetBorcMakbuzlariQueryHandler : IRequestHandler<GetBorcMakbuzlariQuery, Result<PaginatedResult<BorcMakbuzuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetBorcMakbuzlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<BorcMakbuzuDto>>> Handle(GetBorcMakbuzlariQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BorcMakbuzlari
            .Include(x => x.Unit)
            .Include(x => x.GelirTanimi)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BorcluAdi != null && x.BorcluAdi.ToLower().Contains(term)) ||
                (x.Unit != null && x.Unit.DoorNumber.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new BorcMakbuzuDto(
                x.Id, x.EvrakNo, x.IslemTarihi, x.Donem, x.SonOdemeTarihi,
                x.UnitId, x.Unit != null ? x.Unit.DoorNumber : null,
                x.BorcluAdi,
                x.GelirTanimi != null ? x.GelirTanimi.Name : null,
                x.Tutar, x.GecikmeTutari, x.OdenenTutar,
                x.Tutar + x.GecikmeTutari - x.OdenenTutar,
                x.Aciklama, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<BorcMakbuzuDto>>.Success(
            PaginatedResult<BorcMakbuzuDto>.Create(items, total, request.Page, request.PageSize));
    }
}
