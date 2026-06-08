using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Tenancy.RoleTypes.Commands;

public record DeleteRoleTypeCommand(Guid RoleTypeId, Guid SiteId) : IRequest<Result>;

public class DeleteRoleTypeCommandHandler : IRequestHandler<DeleteRoleTypeCommand, Result>
{
    private readonly MasterDbContext _db;

    public DeleteRoleTypeCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteRoleTypeCommand request, CancellationToken cancellationToken)
    {
        var role = await _db.RoleTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoleTypeId && rt.SiteId == request.SiteId, cancellationToken);

        if (role is null)
            return Result.Failure("Rol tipi bulunamadı.");

        if (role.IsDefault)
            return Result.Failure("Varsayılan rol tipi silinemez.");

        var hasUsers = await _db.UserSites.AnyAsync(us => us.RoleTypeId == request.RoleTypeId, cancellationToken);
        if (hasUsers)
            return Result.Failure("Bu rol tipine atanmış kullanıcılar var. Önce kullanıcıları başka bir role atayın.");

        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
