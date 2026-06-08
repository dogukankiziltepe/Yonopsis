using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Queries;

public record GetUnitByIdQuery(Guid Id) : IRequest<Result<UnitDetailDto>>;

public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitDetailDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetUnitByIdQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<UnitDetailDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (unit is null)
            return Result<UnitDetailDto>.Failure("Daire bulunamadı.");

        string? ownerFullName = null;
        string? tenantFullName = null;

        if (unit.OwnerUserId.HasValue)
        {
            var owner = await _masterDb.Users
                .Where(u => u.Id == unit.OwnerUserId.Value)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(cancellationToken);
            if (owner is not null)
                ownerFullName = $"{owner.FirstName} {owner.LastName}";
        }

        if (unit.TenantUserId.HasValue)
        {
            var tenant = await _masterDb.Users
                .Where(u => u.Id == unit.TenantUserId.Value)
                .Select(u => new { u.FirstName, u.LastName })
                .FirstOrDefaultAsync(cancellationToken);
            if (tenant is not null)
                tenantFullName = $"{tenant.FirstName} {tenant.LastName}";
        }

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
            unit.UpdatedAt
        );

        return Result<UnitDetailDto>.Success(dto);
    }
}
