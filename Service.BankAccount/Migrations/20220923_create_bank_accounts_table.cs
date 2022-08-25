using FluentMigrator;

namespace Service.Currency.Migration;

[Migration(20220923230501)]
public class CreateBankAccounts : FluentMigrator.Migration
{
    public const string TableName = "bank_accounts";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("personal_data_id").AsGuid().NotNullable()
            .WithColumn("currency_code").AsString(3).NotNullable()
            .WithColumn("balance").AsDecimal(22, 6).NotNullable()
            .WithColumn("expire_at").AsDate().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}
