using Chassis;
using Service.BankAccount.Model;
using Service.Currency.Migration;

namespace Service.BankAccount.Schema;

public class BankAccountDbSchema
{
    public static string Table => CreateBankAccounts.TableName;

    public static class Columns
    {
        public static string Id { get; }
        public static string PersonalDataId { get; }
        public static string CurrencyCode { get; }
        public static string Balance { get; }
        public static string ExpireAt { get; }

        static Columns()
        {
            Id = nameof(BankAccountDatabaseModel.Id).ToSnakeCase();
            PersonalDataId = nameof(BankAccountDatabaseModel.PersonalDataId).ToSnakeCase();
            CurrencyCode = nameof(BankAccountDatabaseModel.CurrencyCode).ToSnakeCase();
            Balance = nameof(BankAccountDatabaseModel.Balance).ToSnakeCase();
            ExpireAt = nameof(BankAccountDatabaseModel.ExpireAt).ToSnakeCase();
        }
    }
}
