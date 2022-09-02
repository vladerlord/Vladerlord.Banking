using Service.Currency.Abstraction;
using Service.Currency.Exceptions;

namespace Service.Currency.Service;

public class DatabaseCurrencyExchanger : ICurrencyExchanger
{
    private readonly ICurrencyRepository _currencyRepository;

    public DatabaseCurrencyExchanger(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<decimal> Exchange(string fromCurrencyCode, string toCurrencyCode, decimal amount)
    {
        var fromCurrency = await _currencyRepository.FindByCodeAsync(fromCurrencyCode);
        var toCurrency = await _currencyRepository.FindByCodeAsync(toCurrencyCode);

        if (fromCurrency == null || toCurrency == null) throw new EntityNotFoundException();

        var amountAsUsd = fromCurrency.ExchangeRateToUsd * amount;

        return decimal.Round(amountAsUsd / toCurrency.ExchangeRateToUsd, 6);
    }
}
