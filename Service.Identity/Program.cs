using System.Reflection;
using Chassis;
using Dapper;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using Serilog;
using Service.Identity;
using Service.Identity.Abstractions;
using Service.Identity.Repository;
using Service.Identity.Services;

var builder = WebApplication.CreateBuilder(args);
var identityConnectionString = EnvironmentUtils.GetRequiredEnvironmentVariable("IDENTITY_DB_CONNECTION");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
DefaultTypeMap.MatchNamesWithUnderscores = true;

StartupUtils.ConfigureLogging(builder);
Chassis.Grpc.StartupUtils.ConfigureDapperContextBuilder(builder, identityConnectionString);
Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingBuilder(builder);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorBuilder(builder, identityConnectionString,
    Assembly.GetExecutingAssembly());
BindSystemServices(builder.Services);
BindAppServices(builder.Services);
BindRepositories(builder.Services);
ConfigureRabbitmq();

builder.WebHost.ConfigureKestrel(options =>
{
    // metrics port
    options.ListenAnyIP(5188, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    // grpc port, todo bind to env variable
    options.ListenAnyIP(5074, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingApp(app);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorApp(app);

app.MapGrpcService<IdentityGrpcService>();
app.Run();

void BindSystemServices(IServiceCollection services)
{
    services.AddCodeFirstGrpc();
    services.AddGrpc();

    services.AddSingleton(Log.Logger);
}

void BindRepositories(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IConfirmationLinkRepository, ConfirmationLinkRepository>();
}

void BindAppServices(IServiceCollection services)
{
    var jwtSecret = EnvironmentUtils.GetRequiredEnvironmentVariable("SECURITY_JWT_SECRET");
    var jwtExpirationMinutes = EnvironmentUtils.GetRequiredEnvironmentVariable("SECURITY_JWT_EXPIRATION_MINUTES");

    services.AddSingleton(new TokenAuthService(jwtSecret, int.Parse(jwtExpirationMinutes)));
    services.AddSingleton<HashService>();
}

void ConfigureRabbitmq()
{
    var host = EnvironmentUtils.GetRequiredEnvironmentVariable("RABBITMQ_HOST");
    var user = EnvironmentUtils.GetRequiredEnvironmentVariable("RABBITMQ_USER");
    var password = EnvironmentUtils.GetRequiredEnvironmentVariable("RABBITMQ_PASSWORD");

    builder.Services.AddMassTransit(busConfig =>
    {
        busConfig.UsingRabbitMq((_, config) =>
        {
            config.Host(host, "/", h =>
            {
                h.Username(user);
                h.Password(password);
            });
        });
    });
}
