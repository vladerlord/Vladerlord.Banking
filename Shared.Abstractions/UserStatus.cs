namespace Shared.Abstractions;

public enum UserStatus
{
    Created,
    Confirmed,
    Approved,
    Admin,
    Atm
}

public static class UserStatusExtensions
{
    public static string AsString(this UserStatus userStatus)
    {
        return userStatus.ToString().ToLower();
    }
}
