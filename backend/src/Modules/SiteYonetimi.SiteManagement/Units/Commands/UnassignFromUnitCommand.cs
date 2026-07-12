using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Units.Services;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record UnassignOwnerFromUnitCommand(Guid UnitId) : IRequest<Result>;
public record UnassignTenantFromUnitCommand(Guid UnitId) : IRequest<Result>;

public class UnassignOwnerFromUnitCommandHandler : IRequestHandler<UnassignOwnerFromUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IPersonUnitHistoryService _historyService;

    public UnassignOwnerFromUnitCommandHandler(SharedTenantDbContext db, IPersonUnitHistoryService historyService)
    {
        _db = db;
        _historyService = historyService;
    }

    public async Task<Result> Handle(UnassignOwnerFromUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.UnitId, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        if (unit.OwnerUserId.HasValue)
            await _historyService.RecordUnassignmentAsync(unit.SiteId, unit.Id, unit.OwnerUserId.Value, UserType.Owner, cancellationToken);

        unit.OwnerUserId = null;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UnassignTenantFromUnitCommandHandler : IRequestHandler<UnassignTenantFromUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IPersonUnitHistoryService _historyService;

    public UnassignTenantFromUnitCommandHandler(SharedTenantDbContext db, IPersonUnitHistoryService historyService)
    {
        _db = db;
        _historyService = historyService;
    }

    public async Task<Result> Handle(UnassignTenantFromUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.UnitId, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        if (unit.TenantUserId.HasValue)
            await _historyService.RecordUnassignmentAsync(unit.SiteId, unit.Id, unit.TenantUserId.Value, UserType.Renter, cancellationToken);

        unit.TenantUserId = null;
        if (unit.Status == UnitStatus.Kiralik)
            unit.Status = UnitStatus.Dolu;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
