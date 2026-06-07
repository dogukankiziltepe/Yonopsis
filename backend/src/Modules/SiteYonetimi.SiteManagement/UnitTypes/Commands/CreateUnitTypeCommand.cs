using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.UnitTypes.DTOs;

namespace SiteYonetimi.SiteManagement.UnitTypes.Commands;

public record CreateUnitTypeCommand(Guid SiteId, CreateUnitTypeDto Dto) : IRequest<Result<Guid>>;

public class CreateUnitTypeCommandHandler : IRequestHandler<CreateUnitTypeCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;

    public CreateUnitTypeCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var unitType = new UnitType
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name
        };

        _db.UnitTypes.Add(unitType);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(unitType.Id);
    }
}

public class CreateUnitTypeDtoValidator : AbstractValidator<CreateUnitTypeDto>
{
    public CreateUnitTypeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Birim tipi adı zorunludur ve 100 karakteri geçemez.");
    }
}
