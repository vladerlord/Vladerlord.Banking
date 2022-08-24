using System.ServiceModel;
using Shared.Grpc.Currency.Contract;

namespace Shared.Grpc.Currency;

[ServiceContract(Name = "ICurrencyGrpcService")]
public interface ICurrencyGrpcService
{
    [OperationContract(Name = "GetAllAsync")]
    Task<GetAllGrpcResponse> GetAllAsync();
}
