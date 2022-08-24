using Service.Currency.Model;

namespace Service.Currency.Abstraction;

public interface ICurrencyRepository
{
    Task<CurrencyDatabaseModel> CreateAsync(CurrencyDatabaseModel model);
    Task UpdateCurrenciesAsync(IEnumerable<CurrencyDatabaseModel> models);
    Task<IEnumerable<CurrencyDatabaseModel>> GetAllAsync();
}
