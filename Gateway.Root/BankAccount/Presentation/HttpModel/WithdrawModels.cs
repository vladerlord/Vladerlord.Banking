using System.Runtime.Serialization;
using Gateway.Root.Shared;
using Shared.Grpc.BankAccount.Contract;

namespace Gateway.Root.BankAccount.Presentation.HttpModel;

[DataContract]
public class WithdrawRequest
{
    [DataMember] public string BankAccountId { get; init; }
    [DataMember] public decimal Amount { get; init; }
    [DataMember] public string Currency { get; init; }

    public WithdrawRequest()
    {
        BankAccountId = string.Empty;
        Currency = string.Empty;
    }

    public TakeFundsGrpcRequest ToGrpcRequest()
    {
        return new TakeFundsGrpcRequest
        {
            BankAccountId = Guid.Parse(BankAccountId),
            Amount = Amount,
            Currency = Currency
        };
    }
}

[DataContract]
public class WithdrawResponse : GatewayHttpResponse
{
}
