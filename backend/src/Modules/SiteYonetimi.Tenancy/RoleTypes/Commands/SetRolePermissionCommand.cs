using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;

namespace SiteYonetimi.Tenancy.RoleTypes.Commands;

public record SetRolePermissionCommand(Guid RoleTypeId, Guid SiteId, SetRolePermissionDto Dto) : IRequest<Result>;

public class SetRolePermissionCommandHandler : IRequestHandler<SetRolePermissionCommand, Result>
{
    private readonly MasterDbContext _db;

    public SetRolePermissionCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(SetRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var role = await _db.RoleTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoleTypeId && rt.SiteId == request.SiteId, cancellationToken);

        if (role is null)
            return Result.Failure("Rol tipi bulunamadı.");

        if (role.IsDefault)
            return Result.Failure("Varsayılan rol tipinin izinleri değiştirilemez.");

        var existing = await _db.RolePermissions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(rp => rp.RoleTypeId == request.RoleTypeId && rp.PageId == request.Dto.PageId, cancellationToken);

        if (existing is null)
        {
            _db.RolePermissions.Add(new RolePermission
            {
                RoleTypeId = request.RoleTypeId,
                PageId = request.Dto.PageId,
                PermissionLevel = request.Dto.PermissionLevel
            });
        }
        else
        {
            existing.PermissionLevel = request.Dto.PermissionLevel;
            existing.IsDeleted = false;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
