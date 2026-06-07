using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Auth.DTOs;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Auth.Commands;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber) : IRequest<Result<Guid>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;

    public RegisterCommandHandler(MasterDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists)
            return Result<Guid>.Failure("Bu e-posta adresi zaten kullanımda.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}
