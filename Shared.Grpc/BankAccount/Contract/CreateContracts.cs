using System.Runtime.Serialization;
using Shared.Grpc.BankAccount.Model;

namespace Shared.Grpc.BankAccount.Contract;

[DataContract]
public class CreateBankAccountGrpcRequest
{
    [DataMember(Order = 1)] public Guid PersonalDataId { get; init; }
    [DataMember(Order = 2)] public string CurrencyCode { get; init; }
    [DataMember(Order = 3)] public string ExpireAt { get; init; }

    public CreateBankAccountGrpcRequest()
    {
        CurrencyCode = string.Empty;
        ExpireAt = string.Empty;
    }
}

[DataContract]
public class CreateBankAccountGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public BankAccountGrpcModel? BankAccount { get; init; }

    public CreateBankAccountGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
