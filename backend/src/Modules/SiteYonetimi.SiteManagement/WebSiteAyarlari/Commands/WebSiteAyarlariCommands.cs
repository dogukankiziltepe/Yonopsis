using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.WebSiteAyarlari.DTOs;

namespace SiteYonetimi.SiteManagement.WebSiteAyarlari.Commands;

public record UpdateAnaSayfaAyarCommand(Guid SiteId, UpdateAnaSayfaAyarDto Dto) : IRequest<Result<bool>>;
public class UpdateAnaSayfaAyarCommandHandler : IRequestHandler<UpdateAnaSayfaAyarCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAnaSayfaAyarCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateAnaSayfaAyarCommand request, CancellationToken ct)
    {
        var entity = await _db.AnaSayfaAyarlari.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.SiteId == request.SiteId && !x.IsDeleted, ct);
        if (entity is null) { entity = new AnaSayfaAyar { SiteId = request.SiteId }; _db.AnaSayfaAyarlari.Add(entity); }
        entity.SiteAdi = request.Dto.SiteAdi; entity.Slogan = request.Dto.Slogan; entity.KisaAciklama = request.Dto.KisaAciklama;
        entity.IletisimTelefon = request.Dto.IletisimTelefon; entity.IletisimEmail = request.Dto.IletisimEmail;
        entity.Adres = request.Dto.Adres; entity.LogoUrl = request.Dto.LogoUrl; entity.KapakFotoUrl = request.Dto.KapakFotoUrl;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record UpdateSiteTemasCommand(Guid SiteId, UpdateSiteTemasDto Dto) : IRequest<Result<bool>>;
public class UpdateSiteTemasCommandHandler : IRequestHandler<UpdateSiteTemasCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateSiteTemasCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateSiteTemasCommand request, CancellationToken ct)
    {
        var entity = await _db.SiteTemalari.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.SiteId == request.SiteId && !x.IsDeleted, ct);
        if (entity is null) { entity = new SiteTemasi { SiteId = request.SiteId }; _db.SiteTemalari.Add(entity); }
        entity.PrimaryColor = request.Dto.PrimaryColor; entity.SecondaryColor = request.Dto.SecondaryColor;
        entity.AccentColor = request.Dto.AccentColor; entity.LogoUrl = request.Dto.LogoUrl;
        entity.FaviconUrl = request.Dto.FaviconUrl; entity.FontFamily = request.Dto.FontFamily;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
