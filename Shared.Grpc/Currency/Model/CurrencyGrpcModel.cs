using System.Runtime.Serialization;

namespace Shared.Grpc.Currency.Model;

[DataContract]
public class CurrencyGrpcModel
{
    [DataMember(Order = 1)] public string Code { get; init; }
    [DataMember(Order = 2)] public decimal ExchangeRateToUsd { get; init; }

    public CurrencyGrpcModel()
    {
        Code = string.Empty;
    }
}
