using Service.Currency.Model;

namespace Service.Currency.Service;

public class CurrencyService
{
    public decimal Exchange(CurrencyDatabaseModel from, CurrencyDatabaseModel to, decimal amount)
    {
        var amountAsUsd = from.ExchangeRateToUsd * amount;

        return decimal.Round(amountAsUsd / to.ExchangeRateToUsd, 6);
    }
}
