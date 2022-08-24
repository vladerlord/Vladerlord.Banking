using Chassis;
using Service.Currency.Migration;
using Service.Currency.Model;

namespace Service.Currency.Schema;

public class CurrencyDbSchema
{
    public static string Table => CreateCurrencyTable.TableName;

    public static class Columns
    {
        public static string Code { get; }
        public static string ExchangeRateToUsd { get; }

        static Columns()
        {
            Code = nameof(CurrencyDatabaseModel.Code).ToSnakeCase();
            ExchangeRateToUsd = nameof(CurrencyDatabaseModel.ExchangeRateToUsd).ToSnakeCase();
        }
    }
}
