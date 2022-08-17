using Chassis.Grpc;
using Dapper;
using Service.Identity.Abstractions;
using Service.Identity.Models;
using Service.Identity.Scheme;
using Shared.Abstractions;

namespace Service.Identity.Repository;

public class UserRepository : IUserRepository
{
	private readonly DapperContext _context;

	public UserRepository(DapperContext dapperContext)
	{
		_context = dapperContext;
	}

	public async Task<UserDatabaseModel> CreateAsync(UserDatabaseModel userDatabaseModel)
	{
		var createSql = $@"
            INSERT INTO {UserDatabaseSchema.Table}
            ({UserDatabaseSchema.Columns.Id},
            {UserDatabaseSchema.Columns.Email},
            {UserDatabaseSchema.Columns.Password},
            {UserDatabaseSchema.Columns.Status})
            VALUES (@Id, @Email, @Password, @Status)";

		using var connection = _context.CreateConnection();

		await connection.ExecuteAsync(createSql, userDatabaseModel);

		return userDatabaseModel;
	}

	public async Task<UserDatabaseModel?> FindByIdAsync(Guid id)
	{
		var sql = $@"SELECT * FROM {UserDatabaseSchema.Table} WHERE {UserDatabaseSchema.Columns.Id} = @Id";

		using var connection = _context.CreateConnection();

		var entity = await connection.QueryFirstOrDefaultAsync<UserDatabaseModel>(sql, new
		{
			Id = id
		});

		return entity;
	}

	public async Task<UserDatabaseModel?> FindByEmailAsync(string email)
	{
		var sql = $@"SELECT * FROM {UserDatabaseSchema.Table} WHERE {UserDatabaseSchema.Columns.Email} = @Email";

		using var connection = _context.CreateConnection();

		var entity = await connection.QueryFirstOrDefaultAsync<UserDatabaseModel>(sql, new
		{
			Email = email
		});

		return entity;
	}

	public async Task UpdateStatusAsync(Guid id, UserStatus status)
	{
		var sql = $@"UPDATE {UserDatabaseSchema.Table}
			SET {UserDatabaseSchema.Columns.Status} = @Status
			WHERE {UserDatabaseSchema.Columns.Id} = @Id";

		using var connection = _context.CreateConnection();

		await connection.ExecuteAsync(sql, new
		{
			Id = id,
			Status = status
		});
	}
}