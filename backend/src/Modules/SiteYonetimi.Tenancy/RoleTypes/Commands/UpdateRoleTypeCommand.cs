using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;

namespace SiteYonetimi.Tenancy.RoleTypes.Commands;

public record UpdateRoleTypeCommand(Guid RoleTypeId, Guid SiteId, UpdateRoleTypeDto Dto) : IRequest<Result>;

public class UpdateRoleTypeCommandHandler : IRequestHandler<UpdateRoleTypeCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdateRoleTypeCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateRoleTypeCommand request, CancellationToken cancellationToken)
    {
        var role = await _db.RoleTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoleTypeId && rt.SiteId == request.SiteId, cancellationToken);

        if (role is null)
            return Result.Failure("Rol tipi bulunamadı.");

        if (role.IsDefault)
            return Result.Failure("Varsayılan rol tipi yeniden adlandırılamaz.");

        role.Name = request.Dto.Name;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
