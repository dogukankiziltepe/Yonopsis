using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record UpdatePersonGeneralInfoCommand(Guid UserSiteId, Guid SiteId, UpdatePersonGeneralInfoDto Dto) : IRequest<Result>;

public class UpdatePersonGeneralInfoCommandHandler : IRequestHandler<UpdatePersonGeneralInfoCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdatePersonGeneralInfoCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonGeneralInfoCommand request, CancellationToken cancellationToken)
    {
        var us = await _db.UserSites
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result.Failure("Kişi bulunamadı.");

        us.TaxOffice = request.Dto.TaxOffice;
        us.SecondaryEmail = request.Dto.SecondaryEmail;
        us.Address = request.Dto.Address;
        us.UpdatedAt = DateTime.UtcNow;

        _db.PersonPhones.RemoveRange(us.Phones);
        foreach (var phone in request.Dto.Phones)
        {
            us.Phones.Add(new PersonPhone
            {
                UserSiteId = us.Id,
                PhoneNumber = phone.PhoneNumber,
                Label = phone.Label
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
