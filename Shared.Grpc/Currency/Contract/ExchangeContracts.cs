using System.Runtime.Serialization;

namespace Shared.Grpc.Currency.Contract;

[DataContract]
public class ExchangeGrpcRequest
{
    [DataMember(Order = 1)] public string FromCurrencyCode { get; init; }
    [DataMember(Order = 2)] public string ToCurrencyCode { get; init; }
    [DataMember(Order = 3)] public decimal Amount { get; init; }

    public ExchangeGrpcRequest()
    {
        FromCurrencyCode = string.Empty;
        ToCurrencyCode = string.Empty;
    }
}

[DataContract]
public class ExchangeGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public decimal? Amount { get; init; }

    public ExchangeGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
