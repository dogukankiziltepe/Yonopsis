using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record CreateUnitCommand(Guid SiteId, CreateUnitDto Dto) : IRequest<Result<Guid>>;

public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;

    public CreateUnitCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = new Infrastructure.Entities.Shared.Unit
        {
            SiteId = request.SiteId,
            BuildingId = request.Dto.BuildingId,
            UnitTypeId = request.Dto.UnitTypeId,
            DoorNumber = request.Dto.DoorNumber,
            Code = request.Dto.Code,
            Floor = request.Dto.Floor,
            GrossArea = request.Dto.GrossArea,
            NetArea = request.Dto.NetArea,
            LandShare = request.Dto.LandShare,
            Status = request.Dto.Status,
            MonthlyFee = request.Dto.MonthlyFee,
            ParkingCount = request.Dto.ParkingCount,
            Direction = request.Dto.Direction,
            Internet = request.Dto.Internet,
            HasDask = request.Dto.HasDask,
            Description = request.Dto.Description,
            Code2 = request.Dto.Code2,
            Code3 = request.Dto.Code3,
            DeliveryDate = request.Dto.DeliveryDate,
        };

        _db.Units.Add(unit);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(unit.Id);
    }
}

public class CreateUnitDtoValidator : AbstractValidator<CreateUnitDto>
{
    public CreateUnitDtoValidator()
    {
        RuleFor(x => x.BuildingId).NotEmpty();
        RuleFor(x => x.DoorNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.Floor).MaximumLength(50);
        RuleFor(x => x.Internet).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Code2).MaximumLength(20);
        RuleFor(x => x.Code3).MaximumLength(20);
    }
}
