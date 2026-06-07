using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Auth.Queries;

public record GetUserSitesQuery(Guid UserId) : IRequest<Result<List<UserSiteListDto>>>;

public class GetUserSitesQueryHandler : IRequestHandler<GetUserSitesQuery, Result<List<UserSiteListDto>>>
{
    private readonly MasterDbContext _db;

    public GetUserSitesQueryHandler(MasterDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<UserSiteListDto>>> Handle(GetUserSitesQuery request, CancellationToken cancellationToken)
    {
        var userSites = await _db.UserSites
            .Include(us => us.Site).ThenInclude(s => s.SiteSubscriptions)
            .Include(us => us.RoleType)
            .Where(us => us.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var grouped = userSites
            .Where(us => us.Status == UserSiteStatus.Approved)
            .GroupBy(us => us.SiteId)
            .Select(g =>
            {
                var activeSub = g.First().Site.SiteSubscriptions
                    .Where(ss => ss.IsActive && ss.EndDate >= DateTime.UtcNow)
                    .OrderByDescending(ss => ss.EndDate)
                    .FirstOrDefault();

                var pending = userSites
                    .Where(us => us.SiteId == g.Key && us.Status == UserSiteStatus.Pending)
                    .Select(us => new UserSiteApplicationDto(
                        us.Id,
                        us.UserType?.ToString() ?? string.Empty,
                        us.Status.ToString()))
                    .ToList();

                return new UserSiteListDto(
                    g.Key,
                    g.First().Site.Name,
                    null,
                    g.Select(us => us.UserType?.ToString() ?? string.Empty).Distinct().ToList(),
                    activeSub != null,
                    activeSub?.EndDate,
                    pending);
            })
            .ToList();

        return Result<List<UserSiteListDto>>.Success(grouped);
    }
}
