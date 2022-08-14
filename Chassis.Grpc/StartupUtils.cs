using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.Logging;

namespace Chassis.Grpc;

public static class StartupUtils
{
	public static void ConfigureDapperContextBuilder(WebApplicationBuilder builder, string connection)
	{
		builder.Services.AddSingleton(new DapperContext(connection));
	}

	public static void ConfigurePostgresLoggingBuilder(WebApplicationBuilder builder)
	{
		builder.Services.AddSingleton<SerilogNpgsqlLogger>();
		builder.Services.AddSingleton<INpgsqlLoggingProvider, SerilogNgpsqlLoggingProvider>();
	}

	public static void ConfigurePostgresLoggingApp(WebApplication app)
	{
		var provider = app.Services.GetService<INpgsqlLoggingProvider>();

		if (provider == null)
			throw new Exception("INpgsqlLoggingProvider is not defined for DI");

		NpgsqlLogManager.Provider = provider;

		if (app.Environment.IsDevelopment())
			NpgsqlLogManager.IsParameterLoggingEnabled = true;
	}

	public static void ConfigureFluentMigratorBuilder(WebApplicationBuilder builder, string connection,
		Assembly assembly)
	{
		builder.Services
			.AddFluentMigratorCore()
			.ConfigureRunner(fluentMigratorBuilder => fluentMigratorBuilder
				.AddPostgres()
				.WithGlobalConnectionString(connection)
				.ScanIn(assembly).For.Migrations()
			);
	}

	public static void ConfigureFluentMigratorApp(WebApplication app)
	{
		app.MigrateDatabase();
	}
}