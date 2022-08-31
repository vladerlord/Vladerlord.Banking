using System.Runtime.Serialization;
using Gateway.Root.Shared;
using Shared.Grpc.BankAccount.Contract;

namespace Gateway.Root.BankAccount.Presentation.HttpModel;

[DataContract]
public class TransferRequest
{
    [DataMember] public string FromBankAccountId { get; init; }
    [DataMember] public string ToBankAccountId { get; init; }
    [DataMember] public decimal Amount { get; init; }

    public TransferRequest()
    {
        FromBankAccountId = string.Empty;
        ToBankAccountId = string.Empty;
    }

    public TransferFundsGrpcRequest ToGrpcRequest()
    {
        return new TransferFundsGrpcRequest
        {
            FromBankAccountId = Guid.Parse(FromBankAccountId),
            ToBankAccountId = Guid.Parse(ToBankAccountId),
            Amount = Amount
        };
    }
}

[DataContract]
public class TransferResponse : GatewayHttpResponse
{
}
