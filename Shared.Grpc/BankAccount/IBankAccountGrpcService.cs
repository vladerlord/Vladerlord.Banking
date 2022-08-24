using System.ServiceModel;
using Shared.Grpc.BankAccount.Contract;

namespace Shared.Grpc.BankAccount;

[ServiceContract(Name = "IBankAccountGrpcService")]
public interface IBankAccountGrpcService
{
    [OperationContract(Name = "CreateAsync")]
    Task<CreateBankAccountGrpcResponse> CreateAsync(CreateBankAccountGrpcRequest request);
}
