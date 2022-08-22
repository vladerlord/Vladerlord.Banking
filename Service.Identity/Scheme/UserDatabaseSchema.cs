using Chassis;
using Service.Identity.Models;

namespace Service.Identity.Scheme;

public static class UserDatabaseSchema
{
    public static string Table => "users";

    public static class Columns
    {
        public static string Id { get; } = nameof(UserDatabaseModel.Id).ToSnakeCase();
        public static string Email { get; } = nameof(UserDatabaseModel.Email).ToSnakeCase();
        public static string Password { get; } = nameof(UserDatabaseModel.Password).ToSnakeCase();
        public static string Status { get; } = nameof(UserDatabaseModel.Status).ToSnakeCase();
    }
}
