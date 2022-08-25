using System.Data;
using Chassis.Grpc;
using Dapper;
using Service.Currency.Abstraction;
using Service.Currency.Model;
using Service.Currency.Schema;

namespace Service.Currency.Repository;

public class PostgresCurrencyRepository : ICurrencyRepository
{
    private readonly DapperContext _dapperContext;

    public PostgresCurrencyRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task UpdateCurrenciesAsync(IEnumerable<CurrencyDatabaseModel> models)
    {
        var sql = $@"
    UPDATE {CurrencyDbSchema.Table}
    SET {CurrencyDbSchema.Columns.ExchangeRateToUsd} = @ExchangeRateToUsd
    WHERE {CurrencyDbSchema.Columns.Code} = @Code";

        using var connection = _dapperContext.CreateConnection();
        connection.Open();

        // pros:
        // + performance
        // + dirty read isn't a problem, since there will be no business logic rollbacks
        // + non-repeatable read isn't a problem, since there is no such transactions yet
        // cons:
        // - change level if non-repeatable read becomes a problem
        using var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

        try
        {
            await connection.ExecuteAsync(sql, models, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public async Task<IEnumerable<CurrencyDatabaseModel>> GetAllAsync()
    {
        var sql = $@"SELECT * FROM {CurrencyDbSchema.Table}";

        using var connection = _dapperContext.CreateConnection();

        return await connection.QueryAsync<CurrencyDatabaseModel>(sql);
    }

    public async Task<CurrencyDatabaseModel?> FindByCodeAsync(string code)
    {
        var sql = $@"SELECT * FROM {CurrencyDbSchema.Table} WHERE {CurrencyDbSchema.Columns.Code} = @Code";

        using var connection = _dapperContext.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CurrencyDatabaseModel>(sql, new
        {
            Code = code
        });
    }
}
