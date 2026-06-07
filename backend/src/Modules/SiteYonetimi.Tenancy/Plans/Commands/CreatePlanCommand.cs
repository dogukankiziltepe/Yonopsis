using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.Plans.DTOs;

namespace SiteYonetimi.Tenancy.Plans.Commands;

public record CreatePlanCommand(CreatePlanDto Dto) : IRequest<Result<Guid>>;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;
    public CreatePlanCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = new SubscriptionPlan
        {
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Price = request.Dto.Price,
            IsActive = request.Dto.IsActive
        };

        _db.SubscriptionPlans.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDto>
{
    public CreatePlanDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
