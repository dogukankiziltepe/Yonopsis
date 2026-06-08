using Microsoft.Extensions.Caching.Memory;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Tests.Helpers;

namespace SiteYonetimi.Tests.Permissions;

public class PermissionServiceTests
{
    private static IMemoryCache CreateCache() =>
        new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetUserPermission_NoUserSite_ReturnsUnauthorized()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var svc = new PermissionService(db, CreateCache());

        var result = await svc.GetUserPermissionAsync(Guid.NewGuid(), Guid.NewGuid(), "Binalar");

        Assert.Equal(PermissionLevel.Unauthorized, result);
    }

    [Fact]
    public async Task GetUserPermission_NoRoleType_ReturnsReadOnly()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var userId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        db.UserSites.Add(new UserSite { UserId = userId, SiteId = siteId, RoleTypeId = null, Status = UserSiteStatus.Approved });
        await db.SaveChangesAsync();

        var svc = new PermissionService(db, CreateCache());
        var result = await svc.GetUserPermissionAsync(userId, siteId, "Binalar");

        Assert.Equal(PermissionLevel.ReadOnly, result);
    }

    [Fact]
    public async Task GetUserPermission_DefaultRole_ReturnsFullAccess()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var userId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var roleTypeId = Guid.NewGuid();

        db.RoleTypes.Add(new RoleType { Id = roleTypeId, SiteId = siteId, Name = "SiteAdmin", IsDefault = true });
        db.UserSites.Add(new UserSite { UserId = userId, SiteId = siteId, RoleTypeId = roleTypeId, Status = UserSiteStatus.Approved });
        await db.SaveChangesAsync();

        var svc = new PermissionService(db, CreateCache());
        var result = await svc.GetUserPermissionAsync(userId, siteId, "Binalar");

        Assert.Equal(PermissionLevel.FullAccess, result);
    }

    [Fact]
    public async Task GetUserPermission_SpecificPagePermission_ReturnsCorrectLevel()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var userId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var roleTypeId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var pageId = Guid.NewGuid();

        db.RoleTypes.Add(new RoleType { Id = roleTypeId, SiteId = siteId, Name = "Staff", IsDefault = false });
        db.UserSites.Add(new UserSite { UserId = userId, SiteId = siteId, RoleTypeId = roleTypeId, Status = UserSiteStatus.Approved });
        db.Pages.Add(new Page { Id = pageId, Name = "Binalar", DisplayName = "Binalar", Label = "Binalar", Route = "/buildings", ModuleId = moduleId });
        db.RolePermissions.Add(new RolePermission { RoleTypeId = roleTypeId, PageId = pageId, PermissionLevel = PermissionLevel.ReadAndCreate });
        await db.SaveChangesAsync();

        var svc = new PermissionService(db, CreateCache());
        var result = await svc.GetUserPermissionAsync(userId, siteId, "Binalar");

        Assert.Equal(PermissionLevel.ReadAndCreate, result);
    }
}
