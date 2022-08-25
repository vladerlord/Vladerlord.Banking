using FluentMigrator;

namespace Service.Currency.Migration;

[Migration(20220923160001)]
public class FillBasicCurrencies : FluentMigrator.Migration
{
    public const string TableName = "currency";

    public override void Up()
    {
        Insert.IntoTable(CreateCurrencyTable.TableName).Row(new { code = "USD", exchange_rate_to_usd = 1 });
        Insert.IntoTable(CreateCurrencyTable.TableName).Row(new { code = "EUR", exchange_rate_to_usd = 1.1 });
        Insert.IntoTable(CreateCurrencyTable.TableName).Row(new { code = "UAH", exchange_rate_to_usd = 0.027 });
    }

    public override void Down()
    {
    }
}
