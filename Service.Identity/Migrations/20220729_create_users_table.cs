using FluentMigrator;

namespace Service.Identity.Migrations;

[Migration(20220729183600)]
public class CreateUsersTable: Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("email").AsString(320).NotNullable().Unique()
            .WithColumn("password").AsString(255).NotNullable()
            .WithColumn("iv").AsString(24).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}