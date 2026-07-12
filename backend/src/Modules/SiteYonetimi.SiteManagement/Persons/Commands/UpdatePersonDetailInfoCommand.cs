using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record UpdatePersonDetailInfoCommand(Guid UserSiteId, Guid SiteId, UpdatePersonDetailInfoDto Dto) : IRequest<Result>;

public class UpdatePersonDetailInfoCommandHandler : IRequestHandler<UpdatePersonDetailInfoCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdatePersonDetailInfoCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonDetailInfoCommand request, CancellationToken cancellationToken)
    {
        var us = await _db.UserSites
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result.Failure("Kişi bulunamadı.");

        us.Description = request.Dto.Description;
        us.EducationStatus = request.Dto.EducationStatus;
        us.SchoolOrInstitution = request.Dto.SchoolOrInstitution;
        us.Profession = request.Dto.Profession;
        us.HasPrivateInsurance = request.Dto.HasPrivateInsurance;
        us.IsMartyrOrVeteranRelative = request.Dto.IsMartyrOrVeteranRelative;
        us.PetType = request.Dto.PetType;
        us.PetDetail = request.Dto.PetDetail;
        us.UpdatedAt = DateTime.UtcNow;

        if (request.Dto.Gender.HasValue)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == us.UserId, cancellationToken);
            if (user is not null)
                user.Gender = request.Dto.Gender.Value;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
