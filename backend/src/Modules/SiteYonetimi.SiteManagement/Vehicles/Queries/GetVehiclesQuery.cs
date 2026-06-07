using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Queries;

public record GetVehiclesBySiteQuery(Guid SiteId) : IRequest<Result<List<VehicleSummaryDto>>>;
public record GetVehiclesByUserQuery(Guid UserId, Guid SiteId) : IRequest<Result<List<VehicleSummaryDto>>>;
public record GetVehicleByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<VehicleSummaryDto>>;

public class GetVehiclesBySiteQueryHandler : IRequestHandler<GetVehiclesBySiteQuery, Result<List<VehicleSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVehiclesBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<VehicleSummaryDto>>> Handle(GetVehiclesBySiteQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Vehicles
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Plate)
            .Select(x => new VehicleSummaryDto(
                x.Id, x.SiteId, x.UserId, x.Plate,
                x.Brand, x.Model, x.Color, x.Year, x.IsActive, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleSummaryDto>>.Success(items);
    }
}

public class GetVehiclesByUserQueryHandler : IRequestHandler<GetVehiclesByUserQuery, Result<List<VehicleSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVehiclesByUserQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<VehicleSummaryDto>>> Handle(GetVehiclesByUserQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Vehicles
            .Where(x => x.SiteId == request.SiteId && x.UserId == request.UserId)
            .OrderBy(x => x.Plate)
            .Select(x => new VehicleSummaryDto(
                x.Id, x.SiteId, x.UserId, x.Plate,
                x.Brand, x.Model, x.Color, x.Year, x.IsActive, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<VehicleSummaryDto>>.Success(items);
    }
}

public class GetVehicleByIdQueryHandler : IRequestHandler<GetVehicleByIdQuery, Result<VehicleSummaryDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetVehicleByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<VehicleSummaryDto>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<VehicleSummaryDto>.Failure("Araç bulunamadı.");

        return Result<VehicleSummaryDto>.Success(new VehicleSummaryDto(
            item.Id, item.SiteId, item.UserId, item.Plate,
            item.Brand, item.Model, item.Color, item.Year, item.IsActive, item.CreatedAt));
    }
}
