using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.IadeMakbuzlari.DTOs;

namespace SiteYonetimi.SiteManagement.IadeMakbuzlari.Queries;

public record GetIadeMakbuzlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<IadeMakbuzuDto>>>;

public class GetIadeMakbuzlariQueryHandler : IRequestHandler<GetIadeMakbuzlariQuery, Result<PaginatedResult<IadeMakbuzuDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetIadeMakbuzlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<IadeMakbuzuDto>>> Handle(GetIadeMakbuzlariQuery request, CancellationToken cancellationToken)
    {
        var query = _db.IadeMakbuzlari
            .Include(x => x.KasaBanka)
            .Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BorcluAdiSnapshot != null && x.BorcluAdiSnapshot.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Tarih)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new IadeMakbuzuDto(
                x.Id, x.EvrakNo, x.Tarih,
                x.BorcluUserId, x.BorcluAdiSnapshot, x.BorcluRol,
                x.KasaBankaId, x.KasaBanka != null ? x.KasaBanka.Name : null,
                x.Tutar, x.Aciklama, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<IadeMakbuzuDto>>.Success(
            PaginatedResult<IadeMakbuzuDto>.Create(items, total, request.Page, request.PageSize));
    }
}
