namespace Service.Currency.Abstraction;

public interface ICurrencyExchanger
{
    public Task<decimal> Exchange(string fromCurrency, string toCurrency, decimal amount);
}
