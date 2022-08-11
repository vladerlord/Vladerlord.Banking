using FluentMigrator;

namespace Service.Identity.Migrations;

[Migration(20220809183600)]
public class CreateConfirmationLinksTable: Migration
{
    public const string TableName = "confirmation_links";
    
    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("type").AsInt32().NotNullable()
            .WithColumn("confirmation_code").AsString(64).NotNullable().Unique()
            .WithColumn("user_id").AsGuid().ForeignKey(CreateUsersTable.TableName, "id");
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}
