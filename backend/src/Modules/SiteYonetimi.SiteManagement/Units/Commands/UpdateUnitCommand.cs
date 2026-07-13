using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record UpdateUnitCommand(Guid Id, UpdateUnitDto Dto) : IRequest<Result>;

public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public UpdateUnitCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        unit.BuildingId = request.Dto.BuildingId;
        unit.UnitTypeId = request.Dto.UnitTypeId;
        unit.DoorNumber = request.Dto.DoorNumber;
        unit.Code = request.Dto.Code;
        unit.Floor = request.Dto.Floor;
        unit.GrossArea = request.Dto.GrossArea;
        unit.NetArea = request.Dto.NetArea;
        unit.LandShare = request.Dto.LandShare;
        unit.Status = request.Dto.Status;
        unit.MonthlyFee = request.Dto.MonthlyFee;
        unit.ParkingCount = request.Dto.ParkingCount;
        unit.Direction = request.Dto.Direction;
        unit.Internet = request.Dto.Internet;
        unit.HasDask = request.Dto.HasDask;
        unit.Description = request.Dto.Description;
        unit.Code2 = request.Dto.Code2;
        unit.Code3 = request.Dto.Code3;
        unit.DeliveryDate = request.Dto.DeliveryDate;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UpdateUnitDtoValidator : AbstractValidator<UpdateUnitDto>
{
    public UpdateUnitDtoValidator()
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
