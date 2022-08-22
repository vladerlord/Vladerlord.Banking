using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Chassis;

public static class StartupUtils
{
    public static void ConfigureLogging(WebApplicationBuilder builder, LogEventLevel level = LogEventLevel.Information)
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

        config.MinimumLevel.Is(level);

        Log.Logger = config.CreateLogger();

        builder.Host.UseSerilog();
    }
}
