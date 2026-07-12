using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Units.Services;

public interface IPersonUnitHistoryService
{
    Task RecordAssignmentAsync(Guid siteId, Guid unitId, Guid personUserId, UserType role, CancellationToken cancellationToken);
    Task RecordUnassignmentAsync(Guid siteId, Guid unitId, Guid personUserId, UserType role, CancellationToken cancellationToken);
}
