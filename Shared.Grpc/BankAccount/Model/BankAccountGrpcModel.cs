using System.Runtime.Serialization;
using Shared.Abstractions;

namespace Shared.Grpc.BankAccount.Model;

[DataContract]
public class BankAccountGrpcModel
{
    [DataMember(Order = 1)] public Guid Id { get; init; }
    [DataMember(Order = 2)] public Guid PersonalDataId { get; init; }
    [DataMember(Order = 3)] public string CurrencyCode { get; init; }
    [DataMember(Order = 4)] public MoneyValue Balance { get; init; }
    [DataMember(Order = 5)] public string ExpireAt { get; init; }

    public DateOnly ExpireAtProperty
    {
        get => DateOnly.Parse(ExpireAt);
        init => ExpireAt = value.ToString();
    }

    public BankAccountGrpcModel()
    {
        CurrencyCode = string.Empty;
        ExpireAt = string.Empty;
    }
}
