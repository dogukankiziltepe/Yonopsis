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
