using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Services;

public interface IDbConnectionResolver
{
    Task<string> GetConnectionStringAsync(CancellationToken ct = default);
}

public class DbConnectionResolver : IDbConnectionResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MasterDbContext _masterDb;
    private readonly IConfiguration _configuration;

    public DbConnectionResolver(
        IHttpContextAccessor httpContextAccessor,
        MasterDbContext masterDb,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _masterDb = masterDb;
        _configuration = configuration;
    }

    public async Task<string> GetConnectionStringAsync(CancellationToken ct = default)
    {
        var sharedConnStr = _configuration.GetConnectionString("SharedTenantDb")!;

        var siteIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("siteId")?.Value;
        if (!Guid.TryParse(siteIdClaim, out var siteId))
            return sharedConnStr;

        var site = await _masterDb.Sites.FindAsync([siteId], ct);
        if (site is null || site.DbMode == DbMode.Shared || string.IsNullOrEmpty(site.ConnectionString))
            return sharedConnStr;

        return site.ConnectionString;
    }
}
