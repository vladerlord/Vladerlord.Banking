using System.Runtime.Serialization;

namespace Shared.Grpc.BankAccount.Contract;

[DataContract]
public class TransferFundsGrpcRequest
{
    [DataMember(Order = 1)] public Guid FromBankAccountId { get; init; }
    [DataMember(Order = 2)] public Guid ToBankAccountId { get; init; }
    [DataMember(Order = 3)] public decimal Amount { get; init; }
}

[DataContract]
public class TransferFundsGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }

    public TransferFundsGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
