using FluentValidation;
using MediatR;
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
        var entity = new Vehicle
        {
            SiteId = request.SiteId,
            UserId = request.Dto.UserId,
            Plate = request.Dto.Plate.ToUpperInvariant(),
            Brand = request.Dto.Brand,
            Model = request.Dto.Model,
            Color = request.Dto.Color,
            Year = request.Dto.Year
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
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Plate).NotEmpty().MaximumLength(20).WithMessage("Plaka zorunludur ve 20 karakteri geçemez.");
    }
}
