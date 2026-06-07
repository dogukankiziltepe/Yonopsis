using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Buildings.DTOs;

namespace SiteYonetimi.SiteManagement.Buildings.Commands;

public record UpdateBuildingCommand(Guid Id, UpdateBuildingDto Dto) : IRequest<Result>;

public class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public UpdateBuildingCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await _db.Buildings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (building is null)
            return Result.Failure("Bina bulunamadı.");

        building.Name = request.Dto.Name;
        building.IsActive = request.Dto.IsActive;
        building.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class UpdateBuildingDtoValidator : AbstractValidator<UpdateBuildingDto>
{
    public UpdateBuildingDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Bina adı zorunludur ve 100 karakteri geçemez.");
    }
}
