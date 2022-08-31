using System.Runtime.Serialization;

namespace Shared.Grpc.BankAccount.Contract;

[DataContract]
public class AddFundsGrpcRequest
{
    [DataMember(Order = 1)] public Guid BankAccountId { get; init; }
    [DataMember(Order = 2)] public decimal Amount { get; init; }
    [DataMember(Order = 3)] public string Currency { get; init; }

    public AddFundsGrpcRequest()
    {
        Currency = string.Empty;
    }
}

[DataContract]
public class AddFundsGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }

    public AddFundsGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
