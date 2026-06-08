using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;

namespace SiteYonetimi.Tenancy.RoleTypes.Queries;

public record GetRoleTypesBySiteQuery(Guid SiteId) : IRequest<Result<List<RoleTypeDto>>>;

public class GetRoleTypesBySiteQueryHandler : IRequestHandler<GetRoleTypesBySiteQuery, Result<List<RoleTypeDto>>>
{
    private readonly MasterDbContext _db;

    public GetRoleTypesBySiteQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<RoleTypeDto>>> Handle(GetRoleTypesBySiteQuery request, CancellationToken cancellationToken)
    {
        var roles = await _db.RoleTypes
            .Where(rt => rt.SiteId == request.SiteId)
            .OrderByDescending(rt => rt.IsDefault)
            .ThenBy(rt => rt.Name)
            .Select(rt => new RoleTypeDto(rt.Id, rt.Name, rt.IsDefault))
            .ToListAsync(cancellationToken);

        return Result<List<RoleTypeDto>>.Success(roles);
    }
}
