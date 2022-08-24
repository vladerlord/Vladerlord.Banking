using Shared.Abstractions;
using Shared.Grpc.Currency.Model;

namespace Service.Currency.Model;

public class CurrencyDatabaseModel
{
    public string Code { get; }
    public MoneyValue ExchangeRateToUsd { get; }

    public CurrencyDatabaseModel(string code, MoneyValue exchangeRateToUsd)
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
