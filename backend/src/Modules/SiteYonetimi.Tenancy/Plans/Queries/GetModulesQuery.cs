using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Tenancy.Plans.Queries;

public record ModuleDto(Guid Id, string Name, string DisplayName, string? Description, bool IsActive);

public record GetAllModulesQuery : IRequest<Result<List<ModuleDto>>>;

public class GetAllModulesQueryHandler : IRequestHandler<GetAllModulesQuery, Result<List<ModuleDto>>>
{
    private readonly MasterDbContext _db;
    public GetAllModulesQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<ModuleDto>>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await _db.Modules
            .IgnoreQueryFilters()
            .OrderBy(x => x.Name)
            .Select(x => new ModuleDto(x.Id, x.Name, x.DisplayName, x.Description, x.IsActive))
            .ToListAsync(cancellationToken);

        return Result<List<ModuleDto>>.Success(modules);
    }
}
