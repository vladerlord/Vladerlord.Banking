using System.Data;
using Npgsql;

namespace Chassis.Grpc;

public class DapperContext
{
	private readonly string _connectionString;

	public DapperContext(string connectionString)
	{
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}