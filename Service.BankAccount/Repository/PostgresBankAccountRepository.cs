using Chassis.Grpc;
using Dapper;
using Service.BankAccount.Abstraction;
using Service.BankAccount.Model;
using Service.BankAccount.Schema;

namespace Service.BankAccount.Repository;

public class PostgresBankAccountRepository : IBankAccountRepository
{
    private readonly DapperContext _dapperContext;
    
    public PostgresBankAccountRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }
    
    public async Task<BankAccountDatabaseModel> CreateAsync(BankAccountDatabaseModel model)
    {
        var sql = $@"
	INSERT INTO {BankAccountDbSchema.Table}
	({BankAccountDbSchema.Columns.Id},
	{BankAccountDbSchema.Columns.PersonalDataId},
	{BankAccountDbSchema.Columns.CurrencyCode},
	{BankAccountDbSchema.Columns.Balance},
	{BankAccountDbSchema.Columns.ExpireAt})
	VALUES (@Id, @PersonalDataId, @CurrencyCode, @Balance, @ExpireAt)";

        using var connection = _dapperContext.CreateConnection();

        await connection.ExecuteAsync(sql, model);

        return model;
    }
}
