using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Queries;

public record GetMyUnitsAsOwnerQuery(Guid UserId, Guid SiteId) : IRequest<Result<List<UnitSummaryDto>>>;
public record GetMyUnitAsTenantQuery(Guid UserId, Guid SiteId) : IRequest<Result<UnitDetailDto?>>;

public class GetMyUnitsAsOwnerQueryHandler : IRequestHandler<GetMyUnitsAsOwnerQuery, Result<List<UnitSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;

    public GetMyUnitsAsOwnerQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<UnitSummaryDto>>> Handle(GetMyUnitsAsOwnerQuery request, CancellationToken cancellationToken)
    {
        var units = await _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .Where(x => x.SiteId == request.SiteId && x.OwnerUserId == request.UserId)
            .OrderBy(x => x.Building.Name).ThenBy(x => x.DoorNumber)
            .Select(x => new UnitSummaryDto(
                x.Id,
                x.BuildingId,
                x.Building.Name,
                x.UnitTypeId,
                x.UnitType != null ? x.UnitType.Name : null,
                x.DoorNumber,
                x.Code,
                x.Floor,
                x.Status,
                x.MonthlyFee,
                x.HasDask,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<UnitSummaryDto>>.Success(units);
    }
}

public class GetMyUnitAsTenantQueryHandler : IRequestHandler<GetMyUnitAsTenantQuery, Result<UnitDetailDto?>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetMyUnitAsTenantQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<UnitDetailDto?>> Handle(GetMyUnitAsTenantQuery request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .FirstOrDefaultAsync(x => x.SiteId == request.SiteId && x.TenantUserId == request.UserId, cancellationToken);

        if (unit is null)
            return Result<UnitDetailDto?>.Success(null);

        string? ownerFullName = null;
        if (unit.OwnerUserId.HasValue)
        {
            var owner = await _masterDb.Users
                .Where(u => u.Id == unit.OwnerUserId.Value)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(cancellationToken);
            if (owner is not null)
                ownerFullName = $"{owner.FirstName} {owner.LastName}";
        }

        var tenantFullName = await _masterDb.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => $"{u.FirstName} {u.LastName}")
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new UnitDetailDto(
            unit.Id,
            unit.SiteId,
            unit.BuildingId,
            unit.Building?.Name,
            unit.UnitTypeId,
            unit.UnitType?.Name,
            unit.DoorNumber,
            unit.Code,
            unit.Floor,
            unit.GrossArea,
            unit.NetArea,
            unit.LandShare,
            unit.Status,
            unit.MonthlyFee,
            unit.ParkingCount,
            unit.Direction,
            unit.Internet,
            unit.HasDask,
            unit.OwnerUserId,
            ownerFullName,
            unit.TenantUserId,
            tenantFullName,
            unit.Description,
            unit.CreatedAt,
            unit.UpdatedAt);

        return Result<UnitDetailDto?>.Success(dto);
    }
}
