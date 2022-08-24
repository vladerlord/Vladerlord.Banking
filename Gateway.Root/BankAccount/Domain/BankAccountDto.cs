using System.Runtime.Serialization;
using Shared.Grpc.BankAccount.Model;

namespace Gateway.Root.BankAccount.Domain;

[DataContract]
public class BankAccountDto
{
    [DataMember] public Guid Id { get; init; }
    [DataMember] public Guid PersonalDataId { get; init; }
    [DataMember] public string CurrencyCode { get; init; }
    [DataMember] public decimal Balance { get; init; }
    [DataMember] public DateOnly ExpireAt { get; init; }

    public BankAccountDto()
    {
        CurrencyCode = string.Empty;
    }
}

public static class BankAccountDtoExtensions
{
    public static BankAccountDto ToDto(this BankAccountGrpcModel grpcModel)
    {
        return new BankAccountDto
        {
            Id = grpcModel.Id,
            PersonalDataId = grpcModel.PersonalDataId,
            CurrencyCode = grpcModel.CurrencyCode,
            Balance = grpcModel.Balance.GetAsDecimal(),
            ExpireAt = grpcModel.ExpireAtProperty
        };
    }
}
