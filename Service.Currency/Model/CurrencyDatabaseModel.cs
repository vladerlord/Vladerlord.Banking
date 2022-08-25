using Shared.Grpc.Currency.Model;

namespace Service.Currency.Model;

public class CurrencyDatabaseModel
{
    public string Code { get; }
    public decimal ExchangeRateToUsd { get; set; }

    public CurrencyDatabaseModel()
    {
        Code = string.Empty;
    }

    public CurrencyDatabaseModel(string code, decimal exchangeRateToUsd)
    {
        Code = code;
        ExchangeRateToUsd = exchangeRateToUsd;
    }

    public CurrencyGrpcModel ToGrpcModel()
    {
        return new CurrencyGrpcModel
        {
            Code = Code,
            ExchangeRateToUsd = ExchangeRateToUsd
        };
    }
}
