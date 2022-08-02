using Dapper;
using IdentityGrpc.Abstractions;
using IdentityGrpc.Models;

namespace IdentityGrpc.Repository;

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
            {UserDatabaseSchema.Columns.Iv})
            VALUES (@Id, @Email, @Password, @Iv)";

        using var connection = _context.CreateConnection();

        Console.WriteLine($"Column: {UserDatabaseSchema.Columns.Id}");
        Console.WriteLine(userDatabaseModel.Id);
        Console.WriteLine(userDatabaseModel.Email);
        Console.WriteLine(userDatabaseModel.Password);
        Console.WriteLine(userDatabaseModel.Iv);

        await connection.ExecuteAsync(createSql, userDatabaseModel);

        return userDatabaseModel;
    }

    public async Task<UserDatabaseModel?> FindByEmail(string email)
    {
        var sql = $@"SELECT * FROM {UserDatabaseSchema.Table} WHERE {UserDatabaseSchema.Columns.Email} = @Email";

        using var connection = _context.CreateConnection();

        var entity = await connection.QueryFirstOrDefaultAsync<UserDatabaseModel>(sql, new
        {
            Email = email
        });

        return entity;
    }
}