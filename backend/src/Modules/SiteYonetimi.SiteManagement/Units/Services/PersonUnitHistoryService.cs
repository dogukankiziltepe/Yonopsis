using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Units.Services;

public class PersonUnitHistoryService : IPersonUnitHistoryService
{
    private readonly SharedTenantDbContext _db;

    public PersonUnitHistoryService(SharedTenantDbContext db) => _db = db;

    public async Task RecordAssignmentAsync(Guid siteId, Guid unitId, Guid personUserId, UserType role, CancellationToken cancellationToken)
    {
        // Reassignment (e.g. owner overwritten without going through unassign first) closes the
        // previous holder's still-open history row so a unit/role never has two open rows.
        var openRow = await _db.PersonUnitHistories
            .Where(x => x.SiteId == siteId && x.UnitId == unitId && x.Role == role && x.ExitDate == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (openRow is not null)
            openRow.ExitDate = DateTime.UtcNow;

        _db.PersonUnitHistories.Add(new PersonUnitHistory
        {
            SiteId = siteId,
            UnitId = unitId,
            PersonUserId = personUserId,
            Role = role,
            EntryDate = DateTime.UtcNow
        });
    }

    public async Task RecordUnassignmentAsync(Guid siteId, Guid unitId, Guid personUserId, UserType role, CancellationToken cancellationToken)
    {
        var openRow = await _db.PersonUnitHistories
            .Where(x => x.SiteId == siteId && x.UnitId == unitId && x.PersonUserId == personUserId && x.Role == role && x.ExitDate == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (openRow is not null)
            openRow.ExitDate = DateTime.UtcNow;
    }
}
