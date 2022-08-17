using FluentMigrator;

namespace Service.Identity.Migrations;

[Migration(20220813154701)]
public class CreateKycScansTable: Migration
{
    public const string TableName = "kyc_scans";
    
    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("file_name").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("personal_data_id").AsGuid().ForeignKey(CreatePersonalDataTable.TableName, "id")
            .WithColumn("file_extension").AsString(10).NotNullable()
            .WithColumn("original_name").AsString(255).NotNullable()
            .WithColumn("content_type").AsString(50).NotNullable();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}
