namespace Service.Currency.Abstraction;

public interface ICurrencyRateUpdater
{
    Task UpdateAllCurrencyRates();
}
