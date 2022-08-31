using System.Runtime.Serialization;
using Shared.Grpc.BankAccount.Model;

namespace Shared.Grpc.BankAccount.Contract;

[DataContract]
public class GetBankAccountByIdGrpcRequest
{
    [DataMember(Order = 1)] public Guid BankAccountId { get; init; }
}

[DataContract]
public class GetBankAccountByIdGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public BankAccountGrpcModel? BankAccount { get; init; }

    public GetBankAccountByIdGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
