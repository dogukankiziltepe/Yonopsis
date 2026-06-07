using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.Plans.DTOs;

namespace SiteYonetimi.Tenancy.Plans.Commands;

public record UpdatePlanCommand(Guid Id, UpdatePlanDto Dto) : IRequest<Result>;
public record SetPlanModuleCommand(Guid PlanId, SetPlanModuleDto Dto) : IRequest<Result>;

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, Result>
{
    private readonly MasterDbContext _db;
    public UpdatePlanCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SubscriptionPlans.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result.Failure("Plan bulunamadı.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.Price = request.Dto.Price;
        entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class SetPlanModuleCommandHandler : IRequestHandler<SetPlanModuleCommand, Result>
{
    private readonly MasterDbContext _db;
    public SetPlanModuleCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(SetPlanModuleCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.SubscriptionPlanModules.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                x.SubscriptionPlanId == request.PlanId &&
                x.ModuleId == request.Dto.ModuleId, cancellationToken);

        if (request.Dto.IsIncluded)
        {
            if (existing == null)
            {
                _db.SubscriptionPlanModules.Add(new SubscriptionPlanModule
                {
                    SubscriptionPlanId = request.PlanId,
                    ModuleId = request.Dto.ModuleId
                });
            }
            else if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            if (existing != null && !existing.IsDeleted)
            {
                existing.IsDeleted = true;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
