using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Auth.Commands;

public record UpdateProfileDto(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? NationalId,
    Gender? Gender);

public record UpdateProfileCommand(Guid UserId, UpdateProfileDto Dto) : IRequest<Result>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly MasterDbContext _db;
    public UpdateProfileCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.Failure("Kullanıcı bulunamadı.");

        user.FirstName = request.Dto.FirstName;
        user.LastName = request.Dto.LastName;
        user.PhoneNumber = request.Dto.PhoneNumber;
        user.NationalId = request.Dto.NationalId;
        user.Gender = request.Dto.Gender;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
