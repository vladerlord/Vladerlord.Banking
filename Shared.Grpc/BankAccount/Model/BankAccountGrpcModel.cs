using System.Globalization;
using System.Runtime.Serialization;

namespace Shared.Grpc.BankAccount.Model;

[DataContract]
public class BankAccountGrpcModel
{
    [DataMember(Order = 1)] public Guid Id { get; init; }
    [DataMember(Order = 2)] public Guid PersonalDataId { get; init; }
    [DataMember(Order = 3)] public string CurrencyCode { get; init; }
    [DataMember(Order = 4)] public decimal Balance { get; init; }
    [DataMember(Order = 5)] public string ExpireAt { get; init; }

    public DateOnly ExpireAtProperty => DateOnly.ParseExact(ExpireAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);

    public BankAccountGrpcModel()
    {
        CurrencyCode = string.Empty;
        ExpireAt = string.Empty;
    }
}
