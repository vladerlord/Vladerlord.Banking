using Shared.Abstractions;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.BankAccount.Model;

namespace Service.BankAccount.Model;

public class BankAccountDatabaseModel
{
    public Guid Id { get; }
    public Guid PersonalDataId { get; }
    public string CurrencyCode { get; }
    public MoneyValue Balance { get; }
    public DateOnly ExpireAt { get; }

    public BankAccountDatabaseModel(Guid id, Guid personalDataId, string currencyCode, MoneyValue balance,
        DateOnly expireAt)
    {
        Id = id;
        PersonalDataId = personalDataId;
        CurrencyCode = currencyCode;
        Balance = balance;
        ExpireAt = expireAt;
    }

    public BankAccountGrpcModel ToGrpcModel()
    {
        return new BankAccountGrpcModel
        {
            Id = Id,
            PersonalDataId = PersonalDataId,
            CurrencyCode = CurrencyCode,
            Balance = Balance,
            ExpireAt = ExpireAt.ToString()
        };
    }
}

public static class BankAccountDatabaseModelExtension
{
    public static BankAccountDatabaseModel ToDatabaseModel(this CreateBankAccountGrpcRequest request,
        MoneyValue balance)
    {
        return new BankAccountDatabaseModel(
            Guid.NewGuid(),
            request.PersonalDataId,
            request.CurrencyCode,
            balance,
            request.ExpireAtProperty
        );
    }
}