using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Vehicles.DTOs;

namespace SiteYonetimi.SiteManagement.Vehicles.Queries;

public record GetVehiclesBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<VehicleSummaryDto>>>;
public record GetVehiclesByUnitQuery(Guid UnitId, Guid SiteId) : IRequest<Result<List<VehicleSummaryDto>>>;
public record GetVehicleByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<VehicleSummaryDto>>;

public class GetVehiclesBySiteQueryHandler : IRequestHandler<GetVehiclesBySiteQuery, Result<PaginatedResult<VehicleSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVehiclesBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<VehicleSummaryDto>>> Handle(GetVehiclesBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Vehicles.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Plate.ToLower().Contains(term) ||
                (x.Brand != null && x.Brand.ToLower().Contains(term)) ||
                (x.Model != null && x.Model.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Plate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new VehicleSummaryDto(
                x.Id, x.SiteId, x.UnitId,
                x.Unit != null ? x.Unit.DoorNumber : null,
                x.OwnerUserId, x.Plate,
                x.Brand, x.Model, x.Color, x.Year, x.IsActive, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<VehicleSummaryDto>>.Success(
            PaginatedResult<VehicleSummaryDto>.Create(items, totalCount, request.Page, request.PageSize));
    }
}

public class GetVehiclesByUnitQueryHandler : IRequestHandler<GetVehiclesByUnitQuery, Result<List<VehicleSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVehiclesByUnitQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<VehicleSummaryDto>>> Handle(GetVehiclesByUnitQuery request, CancellationToken cancellationToken)
    {
        var unitExists = await _db.Units
            .AnyAsync(u => u.Id == request.UnitId && u.SiteId == request.SiteId, cancellationToken);
        if (!unitExists)
            return Result<List<VehicleSummaryDto>>.Failure("Unit not found.");

        var items = await _db.Vehicles
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId)
            .OrderBy(x => x.Plate)
            .Select(x => new VehicleSummaryDto(
                x.Id, x.SiteId, x.UnitId,
                x.Unit != null ? x.Unit.DoorNumber : null,
                x.OwnerUserId, x.Plate,
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

        if (item is null)
            return Result<VehicleSummaryDto>.Failure("Vehicle not found.");

        string? doorNumber = null;
        if (item.UnitId.HasValue)
        {
            var unit = await _db.Units.FindAsync(item.UnitId.Value);
            doorNumber = unit?.DoorNumber;
        }

        return Result<VehicleSummaryDto>.Success(new VehicleSummaryDto(
            item.Id, item.SiteId, item.UnitId, doorNumber,
            item.OwnerUserId, item.Plate,
            item.Brand, item.Model, item.Color, item.Year, item.IsActive, item.CreatedAt));
    }
}
