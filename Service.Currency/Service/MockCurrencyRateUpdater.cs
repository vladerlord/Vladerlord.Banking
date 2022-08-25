using Service.Currency.Abstraction;

namespace Service.Currency.Service;

public class MockCurrencyRateUpdater : ICurrencyRateUpdater
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger _logger;

    public MockCurrencyRateUpdater(ICurrencyRepository currencyRepository, ILogger<MockCurrencyRateUpdater> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task UpdateAllCurrencyRates()
    {
        var random = new Random();
        var currencies = (await _currencyRepository.GetAllAsync()).ToList();

        _logger.LogInformation("Updating currencies exchange rates");

        foreach (var currencyDatabaseModel in currencies)
        {
            // usd is always 1
            if (currencyDatabaseModel.Code == "USD") continue;

            var multiplier = (1000 + random.Next(-10, 10)) / 1000m;
            var newValue = currencyDatabaseModel.ExchangeRateToUsd * multiplier;

            if (newValue <= 0) continue;

            _logger.LogInformation("Changing {Code}: {Old} to {New}",
                currencyDatabaseModel.Code, currencyDatabaseModel.ExchangeRateToUsd, newValue);

            currencyDatabaseModel.ExchangeRateToUsd = newValue;
        }

        await _currencyRepository.UpdateCurrenciesAsync(currencies);
    }
}
