using Chassis.Grpc;
using Dapper;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Service.PersonalData.Scheme;

namespace Service.PersonalData.Repository;

public class KycScanRepository : IKycScanRepository
{
	private readonly DapperContext _dapperContext;

	public KycScanRepository(DapperContext dapperContext)
	{
		_dapperContext = dapperContext;
	}

	public async Task<KycScanDatabaseModel> CreateAsync(KycScanDatabaseModel model)
	{
		var sql = $@"
	INSERT INTO {KycScanDbSchema.Table}
	({KycScanDbSchema.Columns.PersonalDataId},
	{KycScanDbSchema.Columns.FileName},
	{KycScanDbSchema.Columns.FileExtension},
	{KycScanDbSchema.Columns.OriginalName})
	VALUES (@PersonalDataId, @FileName, @FileExtension, @OriginalName)";

		using var connection = _dapperContext.CreateConnection();

		await connection.ExecuteAsync(sql, model);

		return model;
	}

	public async Task<IEnumerable<KycScanDatabaseModel>> GetByPersonalDataIdAsync(Guid personalDataId)
	{
		var sql = $@"
	SELECT * FROM {KycScanDbSchema.Table} WHERE {KycScanDbSchema.Columns.PersonalDataId} = @PersonalDataId";

		using var connection = _dapperContext.CreateConnection();

		return await connection.QueryAsync<KycScanDatabaseModel>(sql, new
		{
			PersonalDataId = personalDataId
		});
	}

	public async Task DeleteByPersonalDataIdAsync(Guid personalDataId)
	{
		var sql = $@"
	DELETE FROM {KycScanDbSchema.Table} WHERE {KycScanDbSchema.Columns.PersonalDataId} = @PersonalDataId";

		using var connection = _dapperContext.CreateConnection();

		await connection.ExecuteAsync(sql, new
		{
			PersonalDataId = personalDataId
		});
	}
}