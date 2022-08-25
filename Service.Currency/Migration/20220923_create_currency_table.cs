using FluentMigrator;

namespace Service.Currency.Migration;

[Migration(20220923153301)]
public class CreateCurrencyTable : FluentMigrator.Migration
{
    public const string TableName = "currency";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("code").AsString(3).NotNullable().PrimaryKey()
            .WithColumn("exchange_rate_to_usd").AsDecimal(19, 9).NotNullable();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}
