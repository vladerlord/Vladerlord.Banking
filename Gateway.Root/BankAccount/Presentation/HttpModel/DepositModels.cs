using System.Runtime.Serialization;
using Gateway.Root.Shared;
using Shared.Grpc.BankAccount.Contract;

namespace Gateway.Root.BankAccount.Presentation.HttpModel;

[DataContract]
public class DepositRequest
{
    [DataMember] public string BankAccountId { get; init; }
    [DataMember] public decimal Amount { get; init; }
    [DataMember] public string Currency { get; init; }

    public DepositRequest()
    {
        BankAccountId = string.Empty;
        Currency = string.Empty;
    }

    public AddFundsGrpcRequest ToGrpcRequest()
    {
        return new AddFundsGrpcRequest
        {
            BankAccountId = Guid.Parse(BankAccountId),
            Amount = Amount,
            Currency = Currency
        };
    }
}

[DataContract]
public class DepositResponse : GatewayHttpResponse
{
}
