using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Commands;

public record CreateVehicleCommand(Guid SiteId, CreateVehicleDto Dto) : IRequest<Result<Guid>>;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateVehicleCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var plate = dto.Plate.ToUpperInvariant();

        var unit = await _db.Units
            .FirstOrDefaultAsync(u => u.Id == dto.UnitId && u.SiteId == request.SiteId, cancellationToken);
        if (unit is null)
            return Result<Guid>.Failure("Unit not found.");

        var plateExists = await _db.Vehicles
            .AnyAsync(v => v.SiteId == request.SiteId && v.Plate == plate, cancellationToken);
        if (plateExists)
            return Result<Guid>.Failure("This plate is already registered for this site.");

        if (dto.OwnerUserId.HasValue)
        {
            bool assignedToUnit = unit.OwnerUserId == dto.OwnerUserId || unit.TenantUserId == dto.OwnerUserId;
            if (!assignedToUnit)
                return Result<Guid>.Failure("The specified person is not assigned to this unit.");
        }

        var entity = new Vehicle
        {
            SiteId = request.SiteId,
            UnitId = dto.UnitId,
            OwnerUserId = dto.OwnerUserId,
            Plate = plate,
            Brand = dto.Brand,
            Model = dto.Model,
            Color = dto.Color,
            Year = dto.Year
        };

        _db.Vehicles.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{
    public CreateVehicleDtoValidator()
    {
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.Plate).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Brand).MaximumLength(50).When(x => x.Brand != null);
        RuleFor(x => x.Model).MaximumLength(50).When(x => x.Model != null);
        RuleFor(x => x.Color).MaximumLength(30).When(x => x.Color != null);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100).When(x => x.Year.HasValue);
    }
}
