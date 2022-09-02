using System.Runtime.Serialization;
using Gateway.Root.BankAccount.Domain;
using Gateway.Root.Shared;
using Shared.Grpc.BankAccount.Contract;

namespace Gateway.Root.BankAccount.Presentation.HttpModel;

[DataContract]
public class CreateRequest
{
    [DataMember] public string CurrencyCode { get; init; }
    [DataMember] public string ExpireAt { get; init; }

    public CreateRequest()
    {
        CurrencyCode = string.Empty;
        ExpireAt = string.Empty;
    }

    public CreateBankAccountGrpcRequest ToGrpcRequest(Guid personalDataId)
    {
        return new CreateBankAccountGrpcRequest
        {
            PersonalDataId = personalDataId,
            CurrencyCode = CurrencyCode,
            ExpireAt = ExpireAt
        };
    }
}

[DataContract]
public class CreateResponse : GatewayHttpResponse
{
    [DataMember(Order = 1)] public BankAccountDto? BankAccount { get; init; }
}
