using Service.Currency.Model;

namespace Service.Currency.Abstraction;

public interface ICurrencyRepository
{
    Task UpdateCurrenciesAsync(IEnumerable<CurrencyDatabaseModel> models);
    Task<IEnumerable<CurrencyDatabaseModel>> GetAllAsync();
    Task<CurrencyDatabaseModel?> FindByCodeAsync(string code);
}
