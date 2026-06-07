namespace SiteYonetimi.Shared.Enums;

public enum UserType
{
    SuperAdmin = 1,
    Owner = 2,
    Renter = 3,
    Management = 4,
    Admin = 5
}

public enum DbMode
{
    Shared = 1,
    Dedicated = 2
}

public enum PermissionLevel
{
    Unauthorized = 0,
    ReadOnly = 1,
    ReadAndCreate = 2,
    FullAccess = 3
}

public enum UserSiteStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public enum UnitStatus
{
    Bos = 0,
    Dolu = 1,
    Kiralik = 2
}

public enum UnitDirection
{
    Kuzey = 0,
    Guney = 1,
    Dogu = 2,
    Bati = 3,
    Bilinmiyor = 4
}

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}
