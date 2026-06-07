using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record InvitePersonCommand(Guid SiteId, InvitePersonDto Dto) : IRequest<Result<Guid>>;

public class InvitePersonCommandHandler : IRequestHandler<InvitePersonCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;

    public InvitePersonCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(InvitePersonCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (dto.UserType == UserType.Management && dto.RoleTypeId is null)
            return Result<Guid>.Failure("Yönetim kullanıcıları için rol tipi zorunludur.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower().Trim(), cancellationToken);

        if (user is null)
        {
            user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.ToLower().Trim(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Sifre1234"),
                MustChangePassword = true
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var alreadyExists = await _db.UserSites.AnyAsync(
            us => us.UserId == user.Id && us.SiteId == request.SiteId && us.UserType == dto.UserType,
            cancellationToken);

        if (alreadyExists)
            return Result<Guid>.Failure("Bu kullanıcı zaten bu tipte siteye eklenmiş.");

        var userSite = new UserSite
        {
            UserId = user.Id,
            SiteId = request.SiteId,
            UserType = dto.UserType,
            RoleTypeId = dto.RoleTypeId,
            Status = UserSiteStatus.Approved
        };

        _db.UserSites.Add(userSite);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(userSite.Id);
    }
}

public class InvitePersonCommandValidator : AbstractValidator<InvitePersonCommand>
{
    public InvitePersonCommandValidator()
    {
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100);
    }
}
