using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Auth.Services;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly MasterDbContext _db;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(MasterDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure("E-posta veya şifre hatalı.");

        if (!user.IsActive)
            return Result<LoginResponse>.Failure("Hesabınız aktif değil.");

        var accessToken = _tokenService.GenerateLoginToken(user);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        await _db.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new LoginResponse(
            accessToken,
            refreshTokenStr,
            DateTime.UtcNow.AddMinutes(5),
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.IsSuperAdmin,
            user.MustChangePassword));
    }
}
