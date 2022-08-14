using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Chassis;

public static class StartupUtils
{
	public static void ConfigureLogging(WebApplicationBuilder builder)
	{
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		if (environment == null)
			throw new Exception($"ASPNETCORE_ENVIRONMENT is not set");

		var config = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithProperty("Environment", environment)
			.Filter.ByExcluding(c =>
				c.Properties.Any(p =>
				{
					var (_, value) = p;
					return value.ToString().Contains("swagger") || value.ToString().Contains("metrics");
				}))
			.WriteTo.Console();

		if (builder.Environment.IsDevelopment())
			config.MinimumLevel.Debug();

		Log.Logger = config.CreateLogger();

		builder.Host.UseSerilog();
	}
}