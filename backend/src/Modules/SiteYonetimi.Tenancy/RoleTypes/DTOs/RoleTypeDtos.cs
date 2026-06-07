using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Tenancy.RoleTypes.DTOs;

public record RoleTypeDto(
    Guid Id,
    string Name,
    bool IsDefault);

public record RoleTypePermissionsDto(
    Guid RoleTypeId,
    string RoleTypeName,
    bool IsDefault,
    List<PagePermissionDto> Pages);

public record PagePermissionDto(
    Guid PageId,
    string PageName,
    string PageLabel,
    string PageRoute,
    PermissionLevel PermissionLevel);

public record CreateRoleTypeDto(string Name);
public record UpdateRoleTypeDto(string Name);
public record SetRolePermissionDto(Guid PageId, PermissionLevel PermissionLevel);
