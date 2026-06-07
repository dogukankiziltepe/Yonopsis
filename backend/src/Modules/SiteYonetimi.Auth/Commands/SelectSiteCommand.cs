using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Auth.Services;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Auth.Commands;

public record SelectSiteCommand(Guid UserId, Guid SiteId, string UserType) : IRequest<Result<SelectSiteResponse>>;

public class SelectSiteCommandHandler : IRequestHandler<SelectSiteCommand, Result<SelectSiteResponse>>
{
    private readonly MasterDbContext _db;
    private readonly ITokenService _tokenService;

    public SelectSiteCommandHandler(MasterDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<Result<SelectSiteResponse>> Handle(SelectSiteCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserType>(request.UserType, out var userTypeEnum))
            return Result<SelectSiteResponse>.Failure("Geçersiz kullanıcı tipi.");

        var userSite = await _db.UserSites
            .Include(us => us.Site)
            .Include(us => us.RoleType)
            .FirstOrDefaultAsync(us =>
                us.UserId == request.UserId &&
                us.SiteId == request.SiteId &&
                us.UserType == userTypeEnum &&
                us.Status == UserSiteStatus.Approved,
                cancellationToken);

        if (userSite is null)
            return Result<SelectSiteResponse>.Failure("Bu siteye erişim yetkiniz yok.");

        var hasActiveSub = await _db.SiteSubscriptions
            .AnyAsync(ss => ss.SiteId == request.SiteId && ss.IsActive && ss.EndDate >= DateTime.UtcNow, cancellationToken);

        if (!hasActiveSub)
            return Result<SelectSiteResponse>.Failure("Bu sitenin lisansı sona ermiştir.");

        var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user is null)
            return Result<SelectSiteResponse>.Failure("Kullanıcı bulunamadı.");

        // Eski refresh token'ları iptal et
        var oldTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == request.UserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var t in oldTokens)
        {
            t.IsRevoked = true;
            t.RevokedReason = "SiteSelected";
        }

        var accessToken = _tokenService.GenerateSiteToken(user, request.SiteId, request.UserType, userSite.RoleTypeId);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        await _db.SaveChangesAsync(cancellationToken);

        return Result<SelectSiteResponse>.Success(new SelectSiteResponse(
            accessToken,
            refreshTokenStr,
            DateTime.UtcNow.AddMinutes(60),
            request.SiteId,
            userSite.Site.Name,
            request.UserType,
            userSite.RoleTypeId,
            userSite.RoleType?.Name));
    }
}
