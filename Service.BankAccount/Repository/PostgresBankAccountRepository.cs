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

    public async Task<BankAccountDatabaseModel?> FindByIdAsync(Guid id)
    {
        var sql = $@"SELECT * FROM {BankAccountDbSchema.Table} WHERE {BankAccountDbSchema.Columns.Id} = @Id";

        using var connection = _dapperContext.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<BankAccountDatabaseModel>(sql, new
        {
            Id = id
        });
    }

    public async Task AddToBalanceAsync(Guid bankAccountId, decimal amount)
    {
        var sql = $@"
    UPDATE {BankAccountDbSchema.Table}
    SET {BankAccountDbSchema.Columns.Balance} = {BankAccountDbSchema.Columns.Balance} + @Amount
    WHERE {BankAccountDbSchema.Columns.Id} = @Id";

        using var connection = _dapperContext.CreateConnection();
        connection.Open();

        var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                Amount = amount,
                Id = bankAccountId
            }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public async Task TakeFromBalanceAsync(Guid bankAccountId, decimal amount)
    {
        var sql = $@"
    UPDATE {BankAccountDbSchema.Table}
    SET {BankAccountDbSchema.Columns.Balance} = {BankAccountDbSchema.Columns.Balance} - @Amount
    WHERE {BankAccountDbSchema.Columns.Id} = @Id";

        using var connection = _dapperContext.CreateConnection();
        connection.Open();

        var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                Amount = amount,
                Id = bankAccountId
            }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public async Task TransferAsync(Guid fromBankAccountId, Guid toBankAccountId, decimal fromAmount, decimal toAmount)
    {
        var sqlFrom = $@"
    UPDATE {BankAccountDbSchema.Table}
    SET {BankAccountDbSchema.Columns.Balance} = {BankAccountDbSchema.Columns.Balance} - @Amount
    WHERE {BankAccountDbSchema.Columns.Id} = @Id";
        var sqlTo = $@"
    UPDATE {BankAccountDbSchema.Table}
    SET {BankAccountDbSchema.Columns.Balance} = {BankAccountDbSchema.Columns.Balance} + @Amount
    WHERE {BankAccountDbSchema.Columns.Id} = @Id";

        using var connection = _dapperContext.CreateConnection();
        connection.Open();

        var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sqlFrom, new
            {
                Amount = fromAmount,
                Id = fromBankAccountId
            }, transaction);
            await connection.ExecuteAsync(sqlTo, new
            {
                Amount = toAmount,
                Id = toBankAccountId
            }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }
}
