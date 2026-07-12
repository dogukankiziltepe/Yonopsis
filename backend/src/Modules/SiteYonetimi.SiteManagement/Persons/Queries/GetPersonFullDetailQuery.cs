using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Queries;

public record GetPersonFullDetailQuery(Guid UserSiteId, Guid SiteId) : IRequest<Result<PersonFullDetailDto>>;

public class GetPersonFullDetailQueryHandler : IRequestHandler<GetPersonFullDetailQuery, Result<PersonFullDetailDto>>
{
    private const int ContactLogTake = 50;

    private readonly MasterDbContext _masterDb;
    private readonly SharedTenantDbContext _db;

    public GetPersonFullDetailQueryHandler(MasterDbContext masterDb, SharedTenantDbContext db)
    {
        _masterDb = masterDb;
        _db = db;
    }

    public async Task<Result<PersonFullDetailDto>> Handle(GetPersonFullDetailQuery request, CancellationToken cancellationToken)
    {
        var us = await _masterDb.UserSites
            .Include(x => x.User)
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result<PersonFullDetailDto>.Failure("Kişi bulunamadı.");

        var userId = us.UserId;

        var general = new PersonGeneralInfoDto(
            us.UserId,
            us.Id,
            us.User.FirstName,
            us.User.LastName,
            us.User.Email,
            us.User.NationalId,
            us.TaxOffice,
            us.SecondaryEmail,
            us.Address,
            us.Phones.Select(p => new PersonPhoneDto(p.Id, p.PhoneNumber, p.Label)).ToList(),
            us.Status == SiteYonetimi.Shared.Enums.UserSiteStatus.Approved);

        var cariHesap = await _db.HesapPlani
            .Where(x => x.SiteId == request.SiteId && x.PersonId == userId)
            .Select(x => new { x.Id, x.HesapKodu, x.HesapAdi })
            .FirstOrDefaultAsync(cancellationToken);

        var detail = new PersonDetailInfoDto(
            us.Description,
            cariHesap?.Id,
            cariHesap?.HesapKodu,
            cariHesap?.HesapAdi,
            us.User.Gender,
            us.EducationStatus,
            us.SchoolOrInstitution,
            us.Profession,
            us.HasPrivateInsurance,
            us.IsMartyrOrVeteranRelative,
            us.PetType,
            us.PetDetail);

        var identity = new PersonIdentityInfoDto(
            us.Nationality,
            us.IdentitySeriNo,
            us.IdentitySiraNo,
            us.PassportNo,
            us.FatherName,
            us.MotherName,
            us.BirthPlace,
            us.BirthDate,
            us.MaritalStatus,
            us.RegisteredCity,
            us.RegisteredDistrict,
            us.RegisteredNeighborhood,
            us.FamilySiraNo,
            us.KayitSiraNo);

        var units = await _db.PersonUnitHistories
            .Include(x => x.Unit)
            .ThenInclude(x => x.Building)
            .Where(x => x.SiteId == request.SiteId && x.PersonUserId == userId)
            .OrderByDescending(x => x.EntryDate)
            .Select(x => new PersonUnitHistoryDto(
                x.Id,
                x.UnitId,
                x.Unit.DoorNumber,
                x.Unit.Building.Name,
                x.Role,
                x.EntryDate,
                x.ExitDate,
                x.ContactPerson,
                x.Notes,
                x.BankPaymentCode))
            .ToListAsync(cancellationToken);

        var vehicles = await _db.Vehicles
            .Where(x => x.SiteId == request.SiteId && x.OwnerUserId == userId)
            .Select(x => new PersonVehicleDto(x.Id, x.Plate, x.Brand, x.Model, x.Color, x.HgsNo, x.IsActive))
            .ToListAsync(cancellationToken);

        var emailLogs = await _db.EmailLogs
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .OrderByDescending(x => x.SentAt)
            .Take(ContactLogTake)
            .Select(x => new PersonEmailLogDto(x.Id, x.SentAt, x.DeliveredAt, x.ReadAt, x.RecipientEmail, x.Subject, x.Body, x.Status))
            .ToListAsync(cancellationToken);

        var smsLogs = await _db.SmsLogs
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .OrderByDescending(x => x.SentAt)
            .Take(ContactLogTake)
            .Select(x => new PersonSmsLogDto(x.Id, x.SentAt, x.PhoneNumber, x.Message, x.Status))
            .ToListAsync(cancellationToken);

        var whatsappLogs = await _db.WhatsappLogs
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .OrderByDescending(x => x.SentAt)
            .Take(ContactLogTake)
            .Select(x => new PersonWhatsappLogDto(x.Id, x.SentAt, x.PhoneNumber, x.Message, x.Status))
            .ToListAsync(cancellationToken);

        var mobilLogs = await _db.MobilBildirimLogs
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .OrderByDescending(x => x.SentAt)
            .Take(ContactLogTake)
            .Select(x => new PersonMobilBildirimLogDto(x.Id, x.SentAt, x.Message, x.Status))
            .ToListAsync(cancellationToken);

        var contactHistory = new PersonContactHistoryDto(emailLogs, smsLogs, whatsappLogs, mobilLogs);

        var accessCards = await _db.AccessCards
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .Select(x => new PersonAccessCardDto(x.Id, x.CardNumber, x.IssueDate, x.ExpiryDate, x.IsActive))
            .ToListAsync(cancellationToken);

        var supportRequests = await _db.SupportRequests
            .Include(x => x.Unit)
            .Where(x => x.SiteId == request.SiteId && x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PersonSupportRequestDto(x.Id, x.CreatedAt, x.UnitId, x.Unit.DoorNumber, x.Subject, x.Status))
            .ToListAsync(cancellationToken);

        var dto = new PersonFullDetailDto(general, detail, identity, units, vehicles, contactHistory, accessCards, supportRequests);

        return Result<PersonFullDetailDto>.Success(dto);
    }
}
