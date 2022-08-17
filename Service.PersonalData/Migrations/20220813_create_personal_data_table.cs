using FluentMigrator;

namespace Service.Identity.Migrations;

[Migration(20220813154700)]
public class CreatePersonalDataTable: Migration
{
    public const string TableName = "personal_data";
    
    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("user_id").AsGuid().NotNullable().Unique()
            .WithColumn("first_name").AsString(128).NotNullable()
            .WithColumn("last_name").AsString(128).NotNullable()
            .WithColumn("country").AsString(128).NotNullable()
            .WithColumn("city").AsString(128).NotNullable()
            .WithColumn("iv").AsString(24).NotNullable()
            .WithColumn("status").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}
