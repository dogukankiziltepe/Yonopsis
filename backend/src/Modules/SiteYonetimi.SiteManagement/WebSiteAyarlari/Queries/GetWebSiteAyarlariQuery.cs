using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.WebSiteAyarlari.DTOs;

namespace SiteYonetimi.SiteManagement.WebSiteAyarlari.Queries;

public record GetAnaSayfaAyarQuery(Guid SiteId) : IRequest<Result<AnaSayfaAyarDto>>;
public class GetAnaSayfaAyarQueryHandler : IRequestHandler<GetAnaSayfaAyarQuery, Result<AnaSayfaAyarDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnaSayfaAyarQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<AnaSayfaAyarDto>> Handle(GetAnaSayfaAyarQuery request, CancellationToken ct)
    {
        var e = await _db.AnaSayfaAyarlari.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.SiteId == request.SiteId && !x.IsDeleted, ct);
        if (e is null) return Result<AnaSayfaAyarDto>.Success(new AnaSayfaAyarDto(null, null, null, null, null, null, null, null, null));
        return Result<AnaSayfaAyarDto>.Success(new AnaSayfaAyarDto(e.Id, e.SiteAdi, e.Slogan, e.KisaAciklama, e.IletisimTelefon, e.IletisimEmail, e.Adres, e.LogoUrl, e.KapakFotoUrl));
    }
}

public record GetSiteTemasQuery(Guid SiteId) : IRequest<Result<SiteTemAsiDto>>;
public class GetSiteTemasQueryHandler : IRequestHandler<GetSiteTemasQuery, Result<SiteTemAsiDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetSiteTemasQueryHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<SiteTemAsiDto>> Handle(GetSiteTemasQuery request, CancellationToken ct)
    {
        var e = await _db.SiteTemalari.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.SiteId == request.SiteId && !x.IsDeleted, ct);
        if (e is null) return Result<SiteTemAsiDto>.Success(new SiteTemAsiDto(null, null, null, null, null, null, null));
        return Result<SiteTemAsiDto>.Success(new SiteTemAsiDto(e.Id, e.PrimaryColor, e.SecondaryColor, e.AccentColor, e.LogoUrl, e.FaviconUrl, e.FontFamily));
    }
}
