using System.Runtime.Serialization;
using Shared.Grpc.Currency.Model;

namespace Shared.Grpc.Currency.Contract;

[DataContract]
public class GetByCodeGrpcRequest
{
    [DataMember(Order = 1)] public string Code { get; init; }

    public GetByCodeGrpcRequest()
    {
        Code = string.Empty;
    }
}

[DataContract]
public class GetByCodeGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public CurrencyGrpcModel? Currency { get; init; }

    public GetByCodeGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
