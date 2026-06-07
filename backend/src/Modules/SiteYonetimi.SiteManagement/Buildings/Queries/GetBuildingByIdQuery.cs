using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Buildings.DTOs;

namespace SiteYonetimi.SiteManagement.Buildings.Queries;

public record GetBuildingByIdQuery(Guid Id) : IRequest<Result<BuildingDetailDto>>;

public class GetBuildingByIdQueryHandler : IRequestHandler<GetBuildingByIdQuery, Result<BuildingDetailDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetBuildingByIdQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<BuildingDetailDto>> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
    {
        var building = await _db.Buildings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (building is null)
            return Result<BuildingDetailDto>.Failure("Bina bulunamadı.");

        int unitCount = 0;
        try { unitCount = await _db.Units.CountAsync(u => u.BuildingId == building.Id, cancellationToken); } catch { }

        return Result<BuildingDetailDto>.Success(new BuildingDetailDto(
            building.Id,
            building.SiteId,
            building.Name,
            building.IsActive,
            unitCount,
            building.CreatedAt,
            building.UpdatedAt));
    }
}
