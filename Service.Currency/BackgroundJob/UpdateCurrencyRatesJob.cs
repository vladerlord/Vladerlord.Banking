using Quartz;
using Service.Currency.Abstraction;

namespace Service.Currency.BackgroundJob;

public class UpdateCurrencyRatesJob : IJob
{
    private readonly ICurrencyRateUpdater _currencyRateUpdater;

    public UpdateCurrencyRatesJob(ICurrencyRateUpdater currencyRateUpdater)
    {
        _currencyRateUpdater = currencyRateUpdater;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _currencyRateUpdater.UpdateAllCurrencyRates();
    }
}
