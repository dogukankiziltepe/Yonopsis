using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record UpdatePersonIdentityInfoCommand(Guid UserSiteId, Guid SiteId, UpdatePersonIdentityInfoDto Dto) : IRequest<Result>;

public class UpdatePersonIdentityInfoCommandHandler : IRequestHandler<UpdatePersonIdentityInfoCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdatePersonIdentityInfoCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonIdentityInfoCommand request, CancellationToken cancellationToken)
    {
        var us = await _db.UserSites
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result.Failure("Kişi bulunamadı.");

        us.Nationality = request.Dto.Nationality;
        us.IdentitySeriNo = request.Dto.IdentitySeriNo;
        us.IdentitySiraNo = request.Dto.IdentitySiraNo;
        us.PassportNo = request.Dto.PassportNo;
        us.FatherName = request.Dto.FatherName;
        us.MotherName = request.Dto.MotherName;
        us.BirthPlace = request.Dto.BirthPlace;
        us.BirthDate = request.Dto.BirthDate;
        us.MaritalStatus = request.Dto.MaritalStatus;
        us.RegisteredCity = request.Dto.RegisteredCity;
        us.RegisteredDistrict = request.Dto.RegisteredDistrict;
        us.RegisteredNeighborhood = request.Dto.RegisteredNeighborhood;
        us.FamilySiraNo = request.Dto.FamilySiraNo;
        us.KayitSiraNo = request.Dto.KayitSiraNo;
        us.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
