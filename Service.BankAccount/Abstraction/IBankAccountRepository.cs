using Service.BankAccount.Model;

namespace Service.BankAccount.Abstraction;

public interface IBankAccountRepository
{
    Task<BankAccountDatabaseModel> CreateAsync(BankAccountDatabaseModel model);
}
