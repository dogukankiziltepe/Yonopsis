using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Queries;

public record GetPendingPersonsQuery(Guid SiteId) : IRequest<Result<List<PersonDto>>>;

public class GetPendingPersonsQueryHandler : IRequestHandler<GetPendingPersonsQuery, Result<List<PersonDto>>>
{
    private readonly MasterDbContext _db;

    public GetPendingPersonsQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<PersonDto>>> Handle(GetPendingPersonsQuery request, CancellationToken cancellationToken)
    {
        var persons = await _db.UserSites
            .Include(us => us.User)
            .Include(us => us.RoleType)
            .Where(us => us.SiteId == request.SiteId && us.Status == UserSiteStatus.Pending)
            .OrderBy(us => us.CreatedAt)
            .Select(us => new PersonDto(
                us.UserId, us.Id,
                us.User.FirstName, us.User.LastName, us.User.Email, us.User.PhoneNumber,
                us.UserType, us.RoleTypeId,
                us.RoleType != null ? us.RoleType.Name : null,
                us.Status, us.User.IsActive))
            .ToListAsync(cancellationToken);

        return Result<List<PersonDto>>.Success(persons);
    }
}
