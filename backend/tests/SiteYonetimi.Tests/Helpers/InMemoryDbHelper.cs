using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;

namespace SiteYonetimi.Tests.Helpers;

public static class InMemoryDbHelper
{
    public static MasterDbContext CreateMasterDb(string? name = null)
    {
        var opts = new DbContextOptionsBuilder<MasterDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;
        return new MasterDbContext(opts);
    }

    public static SharedTenantDbContext CreateSharedDb(string? name = null)
    {
        var opts = new DbContextOptionsBuilder<SharedTenantDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;
        return new SharedTenantDbContext(opts);
    }
}
