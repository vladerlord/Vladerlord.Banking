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
		{PersonalDataDbSchema.Columns.Iv})
    VALUES (@Id, @UserId, @FirstName, @LastName, @Country, @City, @Iv)
    ON CONFLICT ({PersonalDataDbSchema.Columns.UserId}) DO UPDATE
	SET 
    	{PersonalDataDbSchema.Columns.FirstName} = excluded.{PersonalDataDbSchema.Columns.FirstName},
    	{PersonalDataDbSchema.Columns.LastName} = excluded.{PersonalDataDbSchema.Columns.LastName},
    	{PersonalDataDbSchema.Columns.Country} = excluded.{PersonalDataDbSchema.Columns.Country},
    	{PersonalDataDbSchema.Columns.City} = excluded.{PersonalDataDbSchema.Columns.City},
    	{PersonalDataDbSchema.Columns.Iv} = excluded.{PersonalDataDbSchema.Columns.Iv}
	RETURNING {PersonalDataDbSchema.Columns.Id}
		";

		using var connection = _dapperContext.CreateConnection();

		var id = await connection.ExecuteScalarAsync<Guid>(sql, model);
		model.Id = id;

		return model;
	}
}