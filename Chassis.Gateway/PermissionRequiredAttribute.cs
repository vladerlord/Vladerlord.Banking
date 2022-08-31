using Shared.Abstractions;

namespace Chassis.Gateway;

public class PermissionRequiredAttribute : Attribute
{
    public UserStatus Role { get; }

    public PermissionRequiredAttribute(UserStatus role)
    {
        Role = role;
    }
}
