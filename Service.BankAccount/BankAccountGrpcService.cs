using Service.BankAccount.Abstraction;
using Service.BankAccount.Model;
using Shared.Grpc;
using Shared.Grpc.BankAccount;
using Shared.Grpc.BankAccount.Contract;

namespace Service.BankAccount;

public class BankAccountGrpcService : IBankAccountGrpcService
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public BankAccountGrpcService(IBankAccountRepository bankAccountRepository)
    {
        _bankAccountRepository = bankAccountRepository;
    }

    public async Task<CreateBankAccountGrpcResponse> CreateAsync(CreateBankAccountGrpcRequest request)
    {
        var databaseModel = await _bankAccountRepository.CreateAsync(request.ToDatabaseModel(0m));

        return new CreateBankAccountGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            BankAccount = databaseModel.ToGrpcModel()
        };
    }
}
