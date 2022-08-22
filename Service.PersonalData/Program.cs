using System.Reflection;
using Chassis;
using Dapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using Service.PersonalData;
using Service.PersonalData.Abstractions;
using Service.PersonalData.Repository;
using Service.PersonalData.Services;

var builder = WebApplication.CreateBuilder(args);
var databaseConnectionString = EnvironmentUtils.GetRequiredEnvironmentVariable("PERSONAL_DATA_DB_CONNECTION");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
DefaultTypeMap.MatchNamesWithUnderscores = true;

StartupUtils.ConfigureLogging(builder);
Chassis.Grpc.StartupUtils.ConfigureDapperContextBuilder(builder, databaseConnectionString);
Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingBuilder(builder);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorBuilder(builder, databaseConnectionString,
    Assembly.GetExecutingAssembly());

BindSystemServices(builder.Services);
BindRepositories(builder.Services);
BindAppServices(builder.Services);

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5073, o => o.Protocols = HttpProtocols.Http2); });

var app = builder.Build();

Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingApp(app);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorApp(app);

app.MapGrpcService<PersonalDataGrpcService>();
app.MapGrpcService<KycScanGrpcService>();
app.Run();

void BindSystemServices(IServiceCollection services)
{
    services.AddCodeFirstGrpc();
    services.AddGrpc();
}

void BindRepositories(IServiceCollection services)
{
    services.AddScoped<IPersonalDataRepository, PersonalDataRepository>();
    services.AddScoped<IKycScanRepository, KycScanRepository>();
}

void BindAppServices(IServiceCollection services)
{
    var scansFolder = EnvironmentUtils.GetRequiredEnvironmentVariable("IDENTITY_SCANS_VOLUME_FOLDER");
    var encryptionKey = EnvironmentUtils.GetRequiredEnvironmentVariable("SECURITY_ENCRYPTION_KEY");

    services.AddSingleton(new EncryptionService(encryptionKey));
    services.AddScoped(_ => new KycScansFileService(scansFolder));
    services.AddScoped<KycScansService>();
    services.AddScoped<PersonalDataService>();
    services.AddScoped<PersonalDataEncryptionService>();
    services.AddScoped<KycScanEncryptionService>();
}
