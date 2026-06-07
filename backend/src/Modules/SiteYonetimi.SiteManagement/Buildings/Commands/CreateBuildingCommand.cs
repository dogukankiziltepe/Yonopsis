using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Buildings.DTOs;

namespace SiteYonetimi.SiteManagement.Buildings.Commands;

public record CreateBuildingCommand(Guid SiteId, CreateBuildingDto Dto) : IRequest<Result<Guid>>;

public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;

    public CreateBuildingCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = new Building
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name
        };

        _db.Buildings.Add(building);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(building.Id);
    }
}

public class CreateBuildingDtoValidator : AbstractValidator<CreateBuildingDto>
{
    public CreateBuildingDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Bina adı zorunludur ve 100 karakteri geçemez.");
    }
}
