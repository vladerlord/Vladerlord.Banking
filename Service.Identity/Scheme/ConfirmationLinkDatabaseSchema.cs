using Chassis;
using Service.Identity.Migrations;
using Service.Identity.Models;

namespace Service.Identity.Scheme;

public static class ConfirmationLinkDatabaseSchema
{
    public static string Table => CreateConfirmationLinksTable.TableName;

    public static class Columns
    {
        public static string Id { get; }
        public static string Type { get; }
        public static string ConfirmationCode { get; }
        public static string UserId { get; }

        static Columns()
        {
            Id = nameof(ConfirmationLinkDatabaseModel.Id).ToSnakeCase();
            Type = nameof(ConfirmationLinkDatabaseModel.Type).ToSnakeCase();
            ConfirmationCode = nameof(ConfirmationLinkDatabaseModel.ConfirmationCode).ToSnakeCase();
            UserId = nameof(ConfirmationLinkDatabaseModel.UserId).ToSnakeCase();
        }
    }
}
