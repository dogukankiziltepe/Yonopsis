using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Commands;

public record UpdateVehicleCommand(Guid Id, Guid SiteId, UpdateVehicleDto Dto) : IRequest<Result>;

public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateVehicleCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Araç bulunamadı.");

        entity.Plate = request.Dto.Plate.ToUpperInvariant();
        entity.Brand = request.Dto.Brand;
        entity.Model = request.Dto.Model;
        entity.Color = request.Dto.Color;
        entity.Year = request.Dto.Year;
        entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
