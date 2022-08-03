using System.Reflection;
using Chassis.Grpc;
using FluentMigrator.Runner;
using Service.Identity;
using Service.Identity.Abstractions;
using Service.Identity.Repository;
using Service.Identity.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Npgsql.Logging;
using ProtoBuf.Grpc.Server;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

ConfigureLogging();
AddServices(builder.Services);

builder.Host.UseSerilog();
builder.WebHost.ConfigureKestrel(options =>
{
    // metrics port
    options.ListenAnyIP(5188, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    // grpc port, todo bind to env variable
    options.ListenAnyIP(5074, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

ConfigurePostgres();

app.MapGrpcService<IdentityGrpcService>();

app.Run();

void AddServices(IServiceCollection services)
{
    services.AddCodeFirstGrpc();
    services.AddGrpc();

    var identityConnectionString = Environment.GetEnvironmentVariable("IDENTITY_DB_CONNECTION");
    var jwtSecret = Environment.GetEnvironmentVariable("SECURITY_JWT_SECRET");
    var jwtExpirationMinutes = Environment.GetEnvironmentVariable("SECURITY_JWT_EXPIRATION_MINUTES");
    var encryptionKey = Environment.GetEnvironmentVariable("SECURITY_ENCRYPTION_KEY");

    if (identityConnectionString == null)
        throw new Exception($"env IDENTITY_DB_CONNECTION is not set");

    if (jwtSecret == null || jwtExpirationMinutes == null)
        throw new Exception("env SECURITY_JWT_SECRET or SECURITY_JWT_EXPIRATION_MINUTES is not set");

    if (encryptionKey == null)
        throw new Exception("env SECURITY_ENCRYPTION_KEY is not set");

    services.AddSingleton(new DapperContext(identityConnectionString));
    services.AddSingleton(Log.Logger);
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddSingleton(new TokenAuthService(jwtSecret, int.Parse(jwtExpirationMinutes)));
    services.AddSingleton(new EncryptionService(encryptionKey));

    builder.Services.AddSingleton<SerilogNpgsqlLogger>();
    builder.Services.AddSingleton<INpgsqlLoggingProvider, SerilogNgpsqlLoggingProvider>();

    services
        .AddFluentMigratorCore()
        .ConfigureRunner(fluentMigratorBuilder => fluentMigratorBuilder
            .AddPostgres()
            .WithGlobalConnectionString(identityConnectionString)
            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()
        );
}

void ConfigureLogging()
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
}

void ConfigurePostgres()
{
    var provider = app.Services.GetService<INpgsqlLoggingProvider>();

    if (provider == null)
        throw new Exception("INpgsqlLoggingProvider is not defined for DI");

    NpgsqlLogManager.Provider = provider;

    if (app.Environment.IsDevelopment())
        NpgsqlLogManager.IsParameterLoggingEnabled = true;

    app.MigrateDatabase();
}