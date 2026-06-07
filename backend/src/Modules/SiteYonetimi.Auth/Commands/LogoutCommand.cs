using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly MasterDbContext _db;
    public LogoutCommandHandler(MasterDbContext db) { _db = db; }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);
        if (token is null) return Result.Failure("Token bulunamadi.");
        token.IsRevoked = true;
        token.RevokedReason = "Logout";
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
