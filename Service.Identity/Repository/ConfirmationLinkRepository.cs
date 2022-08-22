using Chassis.Grpc;
using Dapper;
using Service.Identity.Abstractions;
using Service.Identity.Models;
using Service.Identity.Scheme;

namespace Service.Identity.Repository;

public class ConfirmationLinkRepository : IConfirmationLinkRepository
{
    private readonly DapperContext _context;

    public ConfirmationLinkRepository(DapperContext dapperContext)
    {
        _context = dapperContext;
    }

    public async Task<ConfirmationLinkDatabaseModel> CreateAsync(ConfirmationLinkDatabaseModel model)
    {
        var sql = $@"
            INSERT INTO {ConfirmationLinkDatabaseSchema.Table}
            ({ConfirmationLinkDatabaseSchema.Columns.Id},
            {ConfirmationLinkDatabaseSchema.Columns.Type},
            {ConfirmationLinkDatabaseSchema.Columns.ConfirmationCode},
            {ConfirmationLinkDatabaseSchema.Columns.UserId})
            VALUES (@Id, @Type, @ConfirmationCode, @UserId)";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, model);

        return model;
    }

    public async Task<ConfirmationLinkDatabaseModel?> FindByConfirmationCodeAsync(string confirmationCode)
    {
        var sql = $@"
			SELECT * FROM {ConfirmationLinkDatabaseSchema.Table}
			WHERE {ConfirmationLinkDatabaseSchema.Columns.ConfirmationCode} = @ConfirmationCode";

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ConfirmationLinkDatabaseModel>(sql, new
        {
            ConfirmationCode = confirmationCode
        });
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var sql = $@"DELETE FROM {ConfirmationLinkDatabaseSchema.Table}
			WHERE {ConfirmationLinkDatabaseSchema.Columns.Id} = @Id";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            Id = id
        });
    }
}
