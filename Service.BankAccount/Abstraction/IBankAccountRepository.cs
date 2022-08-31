using Service.BankAccount.Model;

namespace Service.BankAccount.Abstraction;

public interface IBankAccountRepository
{
    Task<BankAccountDatabaseModel> CreateAsync(BankAccountDatabaseModel model);
    Task<BankAccountDatabaseModel?> FindByIdAsync(Guid id);
    Task AddToBalanceAsync(Guid bankAccountId, decimal amount);
    Task TakeFromBalanceAsync(Guid bankAccountId, decimal amount);
    Task TransferAsync(Guid fromBankAccountId, Guid toBankAccountId, decimal fromAmount, decimal toAmount);
}
