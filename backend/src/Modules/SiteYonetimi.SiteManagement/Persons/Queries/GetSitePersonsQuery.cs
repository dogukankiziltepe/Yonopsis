using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Queries;

public record GetSitePersonsQuery(Guid SiteId) : IRequest<Result<List<PersonDto>>>;

public class GetSitePersonsQueryHandler : IRequestHandler<GetSitePersonsQuery, Result<List<PersonDto>>>
{
    private readonly MasterDbContext _db;

    public GetSitePersonsQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<PersonDto>>> Handle(GetSitePersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await _db.UserSites
            .Include(us => us.User)
            .Include(us => us.RoleType)
            .Where(us => us.SiteId == request.SiteId)
            .OrderBy(us => us.User.FirstName).ThenBy(us => us.User.LastName)
            .Select(us => new PersonDto(
                us.UserId,
                us.Id,
                us.User.FirstName,
                us.User.LastName,
                us.User.Email,
                us.User.PhoneNumber,
                us.UserType,
                us.RoleTypeId,
                us.RoleType != null ? us.RoleType.Name : null,
                us.Status,
                us.User.IsActive))
            .ToListAsync(cancellationToken);

        return Result<List<PersonDto>>.Success(persons);
    }
}
