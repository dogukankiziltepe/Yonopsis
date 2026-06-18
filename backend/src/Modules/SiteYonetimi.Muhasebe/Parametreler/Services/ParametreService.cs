using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;

namespace SiteYonetimi.Muhasebe.Parametreler.Services;

/// <summary>Tenant başına tek muhasebe parametre kaydını getirir/oluşturur.</summary>
public interface IParametreService
{
    Task<MuhasebeParametre> GetOrCreateAsync(Guid siteId, CancellationToken ct);
}

public class ParametreService : IParametreService
{
    private readonly SharedTenantDbContext _db;

    public ParametreService(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<MuhasebeParametre> GetOrCreateAsync(Guid siteId, CancellationToken ct)
    {
        var p = await _db.MuhasebeParametreler.FirstOrDefaultAsync(x => x.SiteId == siteId, ct);
        if (p is not null) return p;

        p = new MuhasebeParametre { SiteId = siteId };
        _db.MuhasebeParametreler.Add(p);
        await _db.SaveChangesAsync(ct);
        return p;
    }
}
