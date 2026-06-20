using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Commands;

public record ChangeVehicleOwnerCommand(Guid SiteId, Guid VehicleId, ChangeVehicleOwnerDto Dto) : IRequest<Result>;

public class ChangeVehicleOwnerCommandHandler : IRequestHandler<ChangeVehicleOwnerCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public ChangeVehicleOwnerCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(ChangeVehicleOwnerCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _db.Vehicles
            .FirstOrDefaultAsync(v => v.Id == request.VehicleId && v.SiteId == request.SiteId, cancellationToken);
        if (vehicle is null)
            return Result.Failure("Vehicle not found.");

        if (request.Dto.OwnerUserId.HasValue)
        {
            if (vehicle.UnitId is null)
                return Result.Failure("Vehicle has no unit assigned. Assign a unit first.");

            var unit = await _db.Units.FindAsync(vehicle.UnitId.Value);
            bool assignedToUnit = unit?.OwnerUserId == request.Dto.OwnerUserId || unit?.TenantUserId == request.Dto.OwnerUserId;
            if (!assignedToUnit)
                return Result.Failure("The specified person is not assigned to this vehicle's unit.");
        }

        vehicle.OwnerUserId = request.Dto.OwnerUserId;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
