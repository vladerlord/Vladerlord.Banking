using System.Runtime.Serialization;

namespace Shared.Grpc.Currency.Model;

[DataContract]
public class CurrencyGrpcModel
{
    [DataMember] public string Code { get; init; }
    [DataMember] public decimal ExchangeRateToUsd { get; init; }

    public CurrencyGrpcModel()
    {
        Code = string.Empty;
    }
}
