using System.ServiceModel;
using Shared.Grpc.Currency.Contract;

namespace Shared.Grpc.Currency;

[ServiceContract(Name = "ICurrencyGrpcService")]
public interface ICurrencyGrpcService
{
    [OperationContract(Name = "GetAllAsync")]
    Task<GetAllGrpcResponse> GetAllAsync();

    [OperationContract(Name = "GetByCodeAsync")]
    Task<GetByCodeGrpcResponse> GetByCodeAsync(GetByCodeGrpcRequest request);

    [OperationContract(Name = "ExchangeAsync")]
    Task<ExchangeGrpcResponse> ExchangeAsync(ExchangeGrpcRequest request);
}
