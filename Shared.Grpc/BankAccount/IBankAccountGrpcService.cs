using System.ServiceModel;
using Shared.Grpc.BankAccount.Contract;

namespace Shared.Grpc.BankAccount;

[ServiceContract(Name = "IBankAccountGrpcService")]
public interface IBankAccountGrpcService
{
    [OperationContract(Name = "CreateAsync")]
    Task<CreateBankAccountGrpcResponse> CreateAsync(CreateBankAccountGrpcRequest request);
    
    [OperationContract(Name = "GetByIdAsync")]
    Task<GetBankAccountByIdGrpcResponse> GetByIdAsync(GetBankAccountByIdGrpcRequest request);

    [OperationContract(Name = "AddFundsAsync")]
    Task<AddFundsGrpcResponse> AddFundsAsync(AddFundsGrpcRequest request);

    [OperationContract(Name = "TakeFundsAsync")]
    Task<TakeFundsGrpcResponse> TakeFundsAsync(TakeFundsGrpcRequest request);

    [OperationContract(Name = "TransferFundsAsync")]
    Task<TransferFundsGrpcResponse> TransferFundsAsync(TransferFundsGrpcRequest request);
}
