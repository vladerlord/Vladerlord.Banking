using Service.Currency.Abstraction;
using Service.Currency.Model;
using Shared.Abstractions;

namespace Service.Currency.Service;

public class MockCurrencyRateUpdater : ICurrencyRateUpdater
{
    private readonly ICurrencyRepository _currencyRepository;

    public MockCurrencyRateUpdater(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task UpdateAllCurrencyRates()
    {
        var random = new Random();

        var mockData = new List<CurrencyDatabaseModel>
        {
            new("USD", new MoneyValue(1_000_000)),
            new("EUR", new MoneyValue(random.NextInt64(1_000_000, 1_100_000))),
            new("UAH", new MoneyValue(random.NextInt64(0_036_500, 0_037_000))),
        };

        await _currencyRepository.UpdateCurrenciesAsync(mockData);
    }
}
