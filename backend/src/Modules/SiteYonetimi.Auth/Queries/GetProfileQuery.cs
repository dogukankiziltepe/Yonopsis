using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Auth.Queries;

public record ProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? NationalId,
    Gender? Gender,
    bool IsActive,
    DateTime CreatedAt);

public record GetProfileQuery(Guid UserId) : IRequest<Result<ProfileDto>>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDto>>
{
    private readonly MasterDbContext _db;
    public GetProfileQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result<ProfileDto>.Failure("Kullanıcı bulunamadı.");

        return Result<ProfileDto>.Success(new ProfileDto(
            user.Id, user.FirstName, user.LastName, user.Email,
            user.PhoneNumber, user.NationalId, user.Gender,
            user.IsActive, user.CreatedAt));
    }
}
