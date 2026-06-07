using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Auth.Services;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<object>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<object>>
{
    private readonly MasterDbContext _db;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(MasterDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<Result<object>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result<object>.Failure("Geçersiz access token.");

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Result<object>.Failure("Geçersiz token içeriği.");

        var tokenType = principal.FindFirst("tokenType")?.Value;

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.UserId == userId &&
                rt.Token == request.RefreshToken &&
                !rt.IsRevoked &&
                rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (storedToken is null)
            return Result<object>.Failure("Geçersiz veya süresi dolmuş refresh token.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
            return Result<object>.Failure("Kullanıcı bulunamadı.");

        storedToken.IsRevoked = true;
        storedToken.RevokedReason = "Refreshed";

        var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        await _db.SaveChangesAsync(cancellationToken);

        if (tokenType == "site")
        {
            var siteIdClaim = principal.FindFirst("siteId")?.Value;
            var userTypeClaim = principal.FindFirst("userType")?.Value ?? string.Empty;
            var roleTypeIdClaim = principal.FindFirst("roleTypeId")?.Value;

            if (!Guid.TryParse(siteIdClaim, out var siteId))
                return Result<object>.Failure("Geçersiz site token içeriği.");

            Guid? roleTypeId = Guid.TryParse(roleTypeIdClaim, out var rid) ? rid : null;

            var site = await _db.Sites.FindAsync(new object[] { siteId }, cancellationToken);
            var userSite = await _db.UserSites
                .Include(us => us.RoleType)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SiteId == siteId, cancellationToken);

            var newSiteToken = _tokenService.GenerateSiteToken(user, siteId, userTypeClaim, roleTypeId);

            return Result<object>.Success(new SelectSiteResponse(
                newSiteToken,
                newRefreshTokenStr,
                DateTime.UtcNow.AddMinutes(60),
                siteId,
                site?.Name ?? string.Empty,
                userTypeClaim,
                roleTypeId,
                userSite?.RoleType?.Name));
        }
        else
        {
            var newLoginToken = _tokenService.GenerateLoginToken(user);

            return Result<object>.Success(new LoginResponse(
                newLoginToken,
                newRefreshTokenStr,
                DateTime.UtcNow.AddMinutes(5),
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsSuperAdmin,
                user.MustChangePassword));
        }
    }
}
