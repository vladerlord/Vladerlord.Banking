using Service.Currency.Abstraction;
using Service.Currency.Model;

namespace Service.Currency.Service;

public class CurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    
    public CurrencyService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }
    
    public async Task CreateAsync(CurrencyDatabaseModel model)
    {
        await _currencyRepository.CreateAsync(model);
    }
}
