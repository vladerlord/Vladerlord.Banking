using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

    public static void ConfigureOpenTelemetry(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        var serviceName = assembly.GetName().Name ?? string.Empty;
        var serviceVersion = assembly.GetName().Version?.ToString() ?? string.Empty;
        var openTelemetryOtlpUri = EnvironmentUtils.GetRequiredEnvironmentVariable("OPENTELEMETRY_OTLP_URI");

        // tracing
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSource(serviceName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName, serviceVersion: serviceVersion))
                .AddOtlpExporter(options => { options.Endpoint = new Uri(openTelemetryOtlpUri); });
        });

        // filter otpl collection
        services.Configure<AspNetCoreInstrumentationOptions>(options =>
        {
            options.Filter = req => !(req.Request.Path.Equals("/health")
                                      || req.Request.Path.Equals("/metrics")
                                      || req.Request.Path.StartsWithSegments("/swagger"));
            options.RecordException = true;
        });

        // metrics
        services.AddOpenTelemetryMetrics(options =>
        {
            options.AddHttpClientInstrumentation()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddMeter(serviceName)
                .AddOtlpExporter(config => { config.Endpoint = new Uri(openTelemetryOtlpUri); });
        });
    }
}
