using System.Globalization;
using Shared.Grpc.BankAccount.Contract;
using Shared.Grpc.BankAccount.Model;

namespace Service.BankAccount.Model;

public class BankAccountDatabaseModel
{
    public Guid Id { get; }
    public Guid PersonalDataId { get; }
    public string CurrencyCode { get; }
    public decimal Balance { get; }
    public DateOnly ExpireAt { get; }

    public BankAccountDatabaseModel()
    {
        CurrencyCode = string.Empty;
    }

    public BankAccountDatabaseModel(Guid id, Guid personalDataId, string currencyCode, decimal balance,
        string expireAt)
    {
        Id = id;
        PersonalDataId = personalDataId;
        CurrencyCode = currencyCode;
        Balance = balance;
        ExpireAt = DateOnly.ParseExact(expireAt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    }

    public BankAccountGrpcModel ToGrpcModel()
    {
        return new BankAccountGrpcModel
        {
            Id = Id,
            PersonalDataId = PersonalDataId,
            CurrencyCode = CurrencyCode,
            Balance = Balance,
            ExpireAt = ExpireAt.ToString("dd.MM.yyyy")
        };
    }
}

public static class BankAccountDatabaseModelExtension
{
    public static BankAccountDatabaseModel ToDatabaseModel(this CreateBankAccountGrpcRequest request, decimal balance)
    {
        return new BankAccountDatabaseModel(
            Guid.NewGuid(),
            request.PersonalDataId,
            request.CurrencyCode,
            balance,
            request.ExpireAt
        );
    }
}
