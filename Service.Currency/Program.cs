using System.Reflection;
using Chassis;
using Dapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using Quartz;
using Service.Currency;
using Service.Currency.Abstraction;
using Service.Currency.BackgroundJob;
using Service.Currency.Repository;
using Service.Currency.Service;

var builder = WebApplication.CreateBuilder(args);
var identityConnectionString = EnvironmentUtils.GetRequiredEnvironmentVariable("CURRENCY_DB_CONNECTION");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
DefaultTypeMap.MatchNamesWithUnderscores = true;

StartupUtils.ConfigureLogging(builder);
builder.Services.ConfigureOpenTelemetry();

Chassis.Grpc.StartupUtils.ConfigureDapperContextBuilder(builder, identityConnectionString);
Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingBuilder(builder);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorBuilder(builder, identityConnectionString,
    Assembly.GetExecutingAssembly());

BindSystemServices(builder.Services);
BindRepositories(builder.Services);
BindAppServices(builder.Services);

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5077, o => o.Protocols = HttpProtocols.Http2); });

var app = builder.Build();

Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingApp(app);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorApp(app);

app.MapGrpcService<CurrencyGrpcService>();
app.Run();

void BindSystemServices(IServiceCollection services)
{
    services.AddCodeFirstGrpc();
    services.AddGrpc();

    services.AddQuartz(q =>
    {
        var updateCurrencyRateKey = JobKey.Create("update_currency_rate");

        q.UseMicrosoftDependencyInjectionJobFactory();
        q.AddJob<UpdateCurrencyRatesJob>(updateCurrencyRateKey);
        q.AddTrigger(config =>
        {
            config.ForJob(updateCurrencyRateKey)
                .WithIdentity($"{updateCurrencyRateKey.Name}_trigger")
                .WithCronSchedule("0/30 * * * * ?");
        });
    });
    services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
}

void BindRepositories(IServiceCollection services)
{
    services.AddScoped<ICurrencyRepository, PostgresCurrencyRepository>();
}

void BindAppServices(IServiceCollection services)
{
    services.AddScoped<ICurrencyExchanger, DatabaseCurrencyExchanger>();
    services.AddScoped<ICurrencyRateUpdater, MockCurrencyRateUpdater>();
}
