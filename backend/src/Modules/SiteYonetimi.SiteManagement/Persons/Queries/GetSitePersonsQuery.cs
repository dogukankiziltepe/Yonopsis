using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Queries;

public record GetSitePersonsQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<PersonDto>>>;

public class GetSitePersonsQueryHandler : IRequestHandler<GetSitePersonsQuery, Result<PaginatedResult<PersonDto>>>
{
    private readonly MasterDbContext _db;

    public GetSitePersonsQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<PersonDto>>> Handle(GetSitePersonsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.UserSites
            .Include(us => us.User)
            .Include(us => us.RoleType)
            .Where(us => us.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(us =>
                us.User.FirstName.ToLower().Contains(term) ||
                us.User.LastName.ToLower().Contains(term) ||
                us.User.Email.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var persons = await query
            .OrderBy(us => us.User.FirstName).ThenBy(us => us.User.LastName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(us => new PersonDto(
                us.UserId, us.Id,
                us.User.FirstName, us.User.LastName, us.User.Email, us.User.PhoneNumber,
                us.UserType, us.RoleTypeId,
                us.RoleType != null ? us.RoleType.Name : null,
                us.Status, us.User.IsActive))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<PersonDto>>.Success(
            PaginatedResult<PersonDto>.Create(persons, totalCount, request.Page, request.PageSize));
    }
}
