using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.Plans.DTOs;

namespace SiteYonetimi.Tenancy.Plans.Queries;

public record GetAllPlansQuery : IRequest<Result<List<PlanDto>>>;
public record GetPlanByIdQuery(Guid Id) : IRequest<Result<PlanDto>>;

public class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, Result<List<PlanDto>>>
{
    private readonly MasterDbContext _db;
    public GetAllPlansQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<PlanDto>>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _db.SubscriptionPlans
            .IgnoreQueryFilters()
            .Include(x => x.PlanModules)
            .ThenInclude(x => x.Module)
            .OrderBy(x => x.Price)
            .ToListAsync(cancellationToken);

        var result = plans.Select(p => new PlanDto(
            p.Id, p.Name, p.Description, p.Price, p.IsActive,
            p.PlanModules.Where(pm => !pm.IsDeleted).Select(pm => pm.Module.Name).ToList(),
            p.CreatedAt)).ToList();

        return Result<List<PlanDto>>.Success(result);
    }
}

public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanDto>>
{
    private readonly MasterDbContext _db;
    public GetPlanByIdQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _db.SubscriptionPlans
            .IgnoreQueryFilters()
            .Include(x => x.PlanModules)
            .ThenInclude(x => x.Module)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (plan == null)
            return Result<PlanDto>.Failure("Plan bulunamadı.");

        return Result<PlanDto>.Success(new PlanDto(
            plan.Id, plan.Name, plan.Description, plan.Price, plan.IsActive,
            plan.PlanModules.Where(pm => !pm.IsDeleted).Select(pm => pm.Module.Name).ToList(),
            plan.CreatedAt));
    }
}
