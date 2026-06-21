using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AidatKalemleri.DTOs;

namespace SiteYonetimi.SiteManagement.AidatKalemleri.Commands;

public record CreateAidatKalemiCommand(Guid SiteId, CreateAidatKalemiDto Dto) : IRequest<Result<Guid>>;

public class CreateAidatKalemiCommandHandler : IRequestHandler<CreateAidatKalemiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAidatKalemiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateAidatKalemiCommand request, CancellationToken cancellationToken)
    {
        var entity = new AidatKalemi
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Order = request.Dto.Order
        };

        _db.AidatKalemleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateAidatKalemiDtoValidator : AbstractValidator<CreateAidatKalemiDto>
{
    public CreateAidatKalemiDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Kalem adı zorunludur ve 100 karakteri geçemez.");
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
