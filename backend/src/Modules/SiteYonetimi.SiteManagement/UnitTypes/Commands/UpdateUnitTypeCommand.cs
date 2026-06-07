using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.UnitTypes.DTOs;

namespace SiteYonetimi.SiteManagement.UnitTypes.Commands;

public record UpdateUnitTypeCommand(Guid Id, UpdateUnitTypeDto Dto) : IRequest<Result>;

public class UpdateUnitTypeCommandHandler : IRequestHandler<UpdateUnitTypeCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public UpdateUnitTypeCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var unitType = await _db.UnitTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (unitType is null)
            return Result.Failure("Birim tipi bulunamadı.");

        unitType.Name = request.Dto.Name;
        unitType.IsActive = request.Dto.IsActive;
        unitType.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class UpdateUnitTypeDtoValidator : AbstractValidator<UpdateUnitTypeDto>
{
    public UpdateUnitTypeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Birim tipi adı zorunludur ve 100 karakteri geçemez.");
    }
}
