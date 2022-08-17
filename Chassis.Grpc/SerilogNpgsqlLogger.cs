using Microsoft.Extensions.Logging;
using Npgsql.Logging;

namespace Chassis.Grpc;

public class SerilogNpgsqlLogger : NpgsqlLogger
{
	private readonly ILogger _logger;

	public SerilogNpgsqlLogger(ILogger<SerilogNpgsqlLogger> logger)
	{
		_logger = logger;
	}

	public override bool IsEnabled(NpgsqlLogLevel level) => _logger.IsEnabled(ToLoggerLogLevel(level));

	public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception? exception = null)
	{
		_logger.Log(ToLoggerLogLevel(level), exception, "{ConnectorId} : {Msg}", connectorId, msg);
	}

	private static LogLevel ToLoggerLogLevel(NpgsqlLogLevel logLevel)
	{
		return logLevel switch
		{
			NpgsqlLogLevel.Debug => LogLevel.Debug,
			NpgsqlLogLevel.Error => LogLevel.Error,
			NpgsqlLogLevel.Fatal => LogLevel.Critical,
			NpgsqlLogLevel.Info => LogLevel.Information,
			NpgsqlLogLevel.Trace => LogLevel.Trace,
			NpgsqlLogLevel.Warn => LogLevel.Warning,
			_ => LogLevel.None,
		};
	}
}