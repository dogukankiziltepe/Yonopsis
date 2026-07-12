using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Queries;

public record GetPersonellerQuery(
    Guid SiteId,
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedResult<PersonelDto>>>;

public class GetPersonellerQueryHandler : IRequestHandler<GetPersonellerQuery, Result<PaginatedResult<PersonelDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPersonellerQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<PersonelDto>>> Handle(GetPersonellerQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Personeller.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            q = q.Where(x => x.Name.Contains(request.Search) ||
                              (x.Department != null && x.Department.Contains(request.Search)) ||
                              (x.Email != null && x.Email.Contains(request.Search)));
        if (request.IsActive.HasValue)
            q = q.Where(x => x.IsActive == request.IsActive);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PersonelDto(
                x.Id, x.SiteId, x.Name, x.Title, x.Phone, x.Email,
                x.Department, x.StartDate, x.IsActive, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<PersonelDto>>.Success(
            PaginatedResult<PersonelDto>.Create(items, total, request.Page, request.PageSize));
    }
}
