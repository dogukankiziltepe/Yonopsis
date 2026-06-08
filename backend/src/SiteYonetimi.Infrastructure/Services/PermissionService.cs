using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Services;

public interface IPermissionService
{
    Task<PermissionLevel> GetUserPermissionAsync(Guid userId, Guid siteId, string pageName, CancellationToken ct = default);
}

public class PermissionService : IPermissionService
{
    private readonly MasterDbContext _db;
    private readonly IMemoryCache _cache;

    public PermissionService(MasterDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<PermissionLevel> GetUserPermissionAsync(Guid userId, Guid siteId, string pageName, CancellationToken ct = default)
    {
        var cacheKey = $"perm_{userId}_{siteId}_{pageName}";

        if (_cache.TryGetValue(cacheKey, out PermissionLevel cached))
            return cached;

        var userSite = await _db.UserSites
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SiteId == siteId && !us.IsDeleted, ct);

        if (userSite == null)
        {
            _cache.Set(cacheKey, PermissionLevel.Unauthorized, TimeSpan.FromMinutes(1));
            return PermissionLevel.Unauthorized;
        }

        if (userSite.RoleTypeId == null)
        {
            _cache.Set(cacheKey, PermissionLevel.ReadOnly, TimeSpan.FromMinutes(1));
            return PermissionLevel.ReadOnly;
        }

        var isDefaultRole = await _db.RoleTypes
            .AnyAsync(rt => rt.Id == userSite.RoleTypeId.Value && rt.IsDefault, ct);

        if (isDefaultRole)
        {
            _cache.Set(cacheKey, PermissionLevel.FullAccess, TimeSpan.FromMinutes(1));
            return PermissionLevel.FullAccess;
        }

        var permission = await _db.RolePermissions
            .Include(rp => rp.Page)
            .FirstOrDefaultAsync(rp =>
                rp.RoleTypeId == userSite.RoleTypeId &&
                rp.Page.Name == pageName &&
                !rp.IsDeleted, ct);

        var result = permission?.PermissionLevel ?? PermissionLevel.Unauthorized;
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
        return result;
    }
}
