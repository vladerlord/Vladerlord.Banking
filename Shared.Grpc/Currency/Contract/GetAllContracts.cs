using System.Runtime.Serialization;
using Shared.Grpc.Currency.Model;

namespace Shared.Grpc.Currency.Contract;

[DataContract]
public class GetAllGrpcRequest
{
}

[DataContract]
public class GetAllGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public IEnumerable<CurrencyGrpcModel> Currencies { get; init; }

    public GetAllGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
        Currencies = new List<CurrencyGrpcModel>();
    }
}
