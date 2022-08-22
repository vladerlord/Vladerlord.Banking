using Chassis.Grpc;
using Dapper;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Service.PersonalData.Scheme;

namespace Service.PersonalData.Repository;

public class PersonalDataRepository : IPersonalDataRepository
{
    private readonly DapperContext _dapperContext;

    public PersonalDataRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<PersonalDataDatabaseModel> CreateOrUpdateAsync(PersonalDataDatabaseModel model)
    {
        var sql = $@"
	INSERT INTO {PersonalDataDbSchema.Table}
		({PersonalDataDbSchema.Columns.Id},
     	{PersonalDataDbSchema.Columns.UserId},
     	{PersonalDataDbSchema.Columns.FirstName},
     	{PersonalDataDbSchema.Columns.LastName},
     	{PersonalDataDbSchema.Columns.Country},
     	{PersonalDataDbSchema.Columns.City},
		{PersonalDataDbSchema.Columns.Iv},
		{PersonalDataDbSchema.Columns.Status})
    VALUES (@Id, @UserId, @FirstName, @LastName, @Country, @City, @Iv, @Status)
    ON CONFLICT ({PersonalDataDbSchema.Columns.UserId}) DO UPDATE
	SET 
    	{PersonalDataDbSchema.Columns.FirstName} = excluded.{PersonalDataDbSchema.Columns.FirstName},
    	{PersonalDataDbSchema.Columns.LastName} = excluded.{PersonalDataDbSchema.Columns.LastName},
    	{PersonalDataDbSchema.Columns.Country} = excluded.{PersonalDataDbSchema.Columns.Country},
    	{PersonalDataDbSchema.Columns.City} = excluded.{PersonalDataDbSchema.Columns.City},
    	{PersonalDataDbSchema.Columns.Iv} = excluded.{PersonalDataDbSchema.Columns.Iv},
    	{PersonalDataDbSchema.Columns.Status} = excluded.{PersonalDataDbSchema.Columns.Status}
	RETURNING {PersonalDataDbSchema.Columns.Id}";

        using var connection = _dapperContext.CreateConnection();

        var id = await connection.ExecuteScalarAsync<Guid>(sql, model);
        model.Id = id;

        return model;
    }

    public async Task<PersonalDataDatabaseModel?> FindByUserIdAsync(Guid userId)
    {
        var sql = $@"SELECT * FROM {PersonalDataDbSchema.Table} WHERE {PersonalDataDbSchema.Columns.UserId} = @UserId";

        var connection = _dapperContext.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PersonalDataDatabaseModel>(sql, new
        {
            UserId = userId
        });
    }

    public async Task<PersonalDataDatabaseModel?> FindByIdAsync(Guid personalDataId)
    {
        var sql = $@"SELECT * FROM {PersonalDataDbSchema.Table} WHERE {PersonalDataDbSchema.Columns.Id} = @Id";

        var connection = _dapperContext.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PersonalDataDatabaseModel>(sql, new
        {
            Id = personalDataId
        });
    }

    public async Task<List<PersonalDataDatabaseModel>> GetUnapprovedAsync()
    {
        var sql = $@"SELECT * FROM {PersonalDataDbSchema.Table} WHERE {PersonalDataDbSchema.Columns.Status} = @Status";

        var connection = _dapperContext.CreateConnection();

        var result = await connection.QueryAsync<PersonalDataDatabaseModel>(sql, new
        {
            Status = PersonalDataStatus.PendingApproval
        });

        return result.ToList();
    }

    public async Task ChangeStatusAsync(Guid id, PersonalDataStatus status)
    {
        var sql = $@"UPDATE {PersonalDataDbSchema.Table}
	SET {PersonalDataDbSchema.Columns.Status} = @Status
	WHERE {PersonalDataDbSchema.Columns.Id} = @Id";

        var connection = _dapperContext.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Status = status
        });
    }
}
