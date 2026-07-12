using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Rezervasyonlar.DTOs;

namespace SiteYonetimi.SiteManagement.Rezervasyonlar.Queries;

public record GetRezervasyonlarQuery(
    Guid SiteId,
    Guid? TesisId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    RezervasyonDurum? Durum = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedResult<RezervasyonDto>>>;

public class GetRezervasyonlarQueryHandler : IRequestHandler<GetRezervasyonlarQuery, Result<PaginatedResult<RezervasyonDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetRezervasyonlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<RezervasyonDto>>> Handle(GetRezervasyonlarQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Rezervasyonlar
            .Include(x => x.Tesis)
            .Where(x => x.SiteId == request.SiteId);

        if (request.TesisId.HasValue) q = q.Where(x => x.TesisId == request.TesisId);
        if (request.From.HasValue)    q = q.Where(x => x.StartDate >= request.From);
        if (request.To.HasValue)      q = q.Where(x => x.EndDate <= request.To);
        if (request.Durum.HasValue)   q = q.Where(x => x.Durum == request.Durum);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.StartDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new RezervasyonDto(
                x.Id, x.SiteId, x.TesisId,
                x.Tesis != null ? x.Tesis.Name : null,
                x.PersonId, x.StartDate, x.EndDate, x.Durum, x.Notes,
                x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<RezervasyonDto>>.Success(
            PaginatedResult<RezervasyonDto>.Create(items, total, request.Page, request.PageSize));
    }
}
