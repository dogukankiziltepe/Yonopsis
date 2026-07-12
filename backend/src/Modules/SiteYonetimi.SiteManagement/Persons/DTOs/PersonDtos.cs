using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.DTOs;

public record PersonDto(
    Guid UserId,
    Guid UserSiteId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    UserType? UserType,
    Guid? RoleTypeId,
    string? RoleName,
    UserSiteStatus Status,
    bool IsActive);

public record PersonDetailDto(
    Guid UserId,
    Guid UserSiteId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? NationalId,
    Gender? Gender,
    UserType? UserType,
    Guid? RoleTypeId,
    string? RoleName,
    UserSiteStatus Status,
    bool IsActive,
    List<UnitSummaryDto> Units);

public record InvitePersonDto(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    UserType UserType,
    Guid? RoleTypeId);

public record UpdatePersonDto(
    UserType? UserType,
    Guid? RoleTypeId,
    UserSiteStatus? Status);

// ── Kişi Detay Sayfası (tablı yapı) ─────────────────────────────────────

public record PersonPhoneDto(Guid Id, string PhoneNumber, string? Label);

public record PersonGeneralInfoDto(
    Guid UserId,
    Guid UserSiteId,
    string FirstName,
    string LastName,
    string Email,
    string? NationalId,
    string? TaxOffice,
    string? SecondaryEmail,
    string? Address,
    List<PersonPhoneDto> Phones,
    bool IsActive);

public record PersonDetailInfoDto(
    string? Description,
    Guid? CariHesapId,
    string? CariHesapKodu,
    string? CariHesapAdi,
    Gender? Gender,
    EducationStatus? EducationStatus,
    string? SchoolOrInstitution,
    string? Profession,
    bool HasPrivateInsurance,
    bool IsMartyrOrVeteranRelative,
    PetType? PetType,
    string? PetDetail);

public record PersonIdentityInfoDto(
    Nationality? Nationality,
    string? IdentitySeriNo,
    string? IdentitySiraNo,
    string? PassportNo,
    string? FatherName,
    string? MotherName,
    string? BirthPlace,
    DateTime? BirthDate,
    MaritalStatus? MaritalStatus,
    string? RegisteredCity,
    string? RegisteredDistrict,
    string? RegisteredNeighborhood,
    string? FamilySiraNo,
    string? KayitSiraNo);

public record PersonUnitHistoryDto(
    Guid Id,
    Guid UnitId,
    string DoorNumber,
    string? BuildingName,
    UserType Role,
    DateTime EntryDate,
    DateTime? ExitDate,
    string? ContactPerson,
    string? Notes,
    string? BankPaymentCode);

public record PersonVehicleDto(
    Guid Id,
    string Plate,
    string? Brand,
    string? Model,
    string? Color,
    string? HgsNo,
    bool IsActive);

public record PersonEmailLogDto(
    Guid Id,
    DateTime SentAt,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    string RecipientEmail,
    string Subject,
    string? Body,
    string Status);

public record PersonSmsLogDto(Guid Id, DateTime SentAt, string PhoneNumber, string Message, string Status);

public record PersonWhatsappLogDto(Guid Id, DateTime SentAt, string PhoneNumber, string Message, string Status);

public record PersonMobilBildirimLogDto(Guid Id, DateTime SentAt, string Message, string Status);

public record PersonContactHistoryDto(
    List<PersonEmailLogDto> EmailLogs,
    List<PersonSmsLogDto> SmsLogs,
    List<PersonWhatsappLogDto> WhatsappLogs,
    List<PersonMobilBildirimLogDto> MobilBildirimLogs);

public record PersonAccessCardDto(
    Guid Id,
    string CardNumber,
    DateTime IssueDate,
    DateTime? ExpiryDate,
    bool IsActive);

public record PersonSupportRequestDto(
    Guid Id,
    DateTime CreatedAt,
    Guid UnitId,
    string? UnitDoorNumber,
    string Subject,
    SupportRequestStatus Status);

public record PersonFullDetailDto(
    PersonGeneralInfoDto General,
    PersonDetailInfoDto Detail,
    PersonIdentityInfoDto Identity,
    List<PersonUnitHistoryDto> Units,
    List<PersonVehicleDto> Vehicles,
    PersonContactHistoryDto ContactHistory,
    List<PersonAccessCardDto> AccessCards,
    List<PersonSupportRequestDto> SupportRequests);

public record PersonPhoneInputDto(string PhoneNumber, string? Label);

public record UpdatePersonGeneralInfoDto(
    string? TaxOffice,
    string? SecondaryEmail,
    string? Address,
    List<PersonPhoneInputDto> Phones);

public record UpdatePersonDetailInfoDto(
    string? Description,
    Gender? Gender,
    EducationStatus? EducationStatus,
    string? SchoolOrInstitution,
    string? Profession,
    bool HasPrivateInsurance,
    bool IsMartyrOrVeteranRelative,
    PetType? PetType,
    string? PetDetail);

public record UpdatePersonIdentityInfoDto(
    Nationality? Nationality,
    string? IdentitySeriNo,
    string? IdentitySiraNo,
    string? PassportNo,
    string? FatherName,
    string? MotherName,
    string? BirthPlace,
    DateTime? BirthDate,
    MaritalStatus? MaritalStatus,
    string? RegisteredCity,
    string? RegisteredDistrict,
    string? RegisteredNeighborhood,
    string? FamilySiraNo,
    string? KayitSiraNo);
