using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;

namespace SiteYonetimi.Tenancy.RoleTypes.Queries;

public record GetRoleTypePermissionsQuery(Guid RoleTypeId, Guid SiteId) : IRequest<Result<RoleTypePermissionsDto>>;

public class GetRoleTypePermissionsQueryHandler : IRequestHandler<GetRoleTypePermissionsQuery, Result<RoleTypePermissionsDto>>
{
    private readonly MasterDbContext _db;

    public GetRoleTypePermissionsQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<RoleTypePermissionsDto>> Handle(GetRoleTypePermissionsQuery request, CancellationToken cancellationToken)
    {
        var roleType = await _db.RoleTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoleTypeId && rt.SiteId == request.SiteId, cancellationToken);

        if (roleType is null)
            return Result<RoleTypePermissionsDto>.Failure("Rol tipi bulunamadı.");

        var pages = await _db.Pages.ToListAsync(cancellationToken);

        var existingPermissions = await _db.RolePermissions
            .Where(rp => rp.RoleTypeId == request.RoleTypeId)
            .ToDictionaryAsync(rp => rp.PageId, rp => rp.PermissionLevel, cancellationToken);

        var pagePermissions = pages
            .OrderBy(p => p.OrderIndex)
            .Select(p => new PagePermissionDto(
                p.Id,
                p.Name,
                p.Label,
                p.Route,
                roleType.IsDefault
                    ? PermissionLevel.FullAccess
                    : existingPermissions.GetValueOrDefault(p.Id, PermissionLevel.Unauthorized)))
            .ToList();

        return Result<RoleTypePermissionsDto>.Success(new RoleTypePermissionsDto(
            roleType.Id,
            roleType.Name,
            roleType.IsDefault,
            pagePermissions));
    }
}
