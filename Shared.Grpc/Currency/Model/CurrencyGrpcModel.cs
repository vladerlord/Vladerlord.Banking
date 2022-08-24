using System.Runtime.Serialization;
using Shared.Abstractions;

namespace Shared.Grpc.Currency.Model;

[DataContract]
public class CurrencyGrpcModel
{
    [DataMember] public string Code { get; init; }
    [DataMember] public MoneyValue ExchangeRateToUsd { get; init; }

    public CurrencyGrpcModel()
    {
        Code = string.Empty;
    }
}
