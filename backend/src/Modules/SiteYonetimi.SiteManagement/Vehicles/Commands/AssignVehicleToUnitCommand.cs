using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Commands;

public record AssignVehicleToUnitCommand(Guid SiteId, Guid VehicleId, AssignVehicleToUnitDto Dto) : IRequest<Result>;

public class AssignVehicleToUnitCommandHandler : IRequestHandler<AssignVehicleToUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public AssignVehicleToUnitCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(AssignVehicleToUnitCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _db.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.SiteId == request.SiteId, cancellationToken);
        if (vehicle is null)
            return Result.Failure("Vehicle not found.");

        var unit = await _db.Units
            .FirstOrDefaultAsync(u => u.Id == request.Dto.UnitId && u.SiteId == request.SiteId, cancellationToken);
        if (unit is null)
            return Result.Failure("Unit not found.");

        if (request.Dto.OwnerUserId.HasValue)
        {
            bool assignedToUnit = unit.OwnerUserId == request.Dto.OwnerUserId || unit.TenantUserId == request.Dto.OwnerUserId;
            if (!assignedToUnit)
                return Result.Failure("The specified person is not assigned to this unit.");
        }

        vehicle.UnitId = request.Dto.UnitId;
        vehicle.OwnerUserId = request.Dto.OwnerUserId;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
