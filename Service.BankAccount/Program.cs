using System.Reflection;
using Chassis;
using Chassis.Grpc.DapperDataType;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using Service.BankAccount;
using Service.BankAccount.Abstraction;
using Service.BankAccount.Repository;

var builder = WebApplication.CreateBuilder(args);
var identityConnectionString = EnvironmentUtils.GetRequiredEnvironmentVariable("BANK_ACCOUNT_DB_CONNECTION");

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
DefaultTypeMap.MatchNamesWithUnderscores = true;

StartupUtils.ConfigureLogging(builder);
Chassis.Grpc.StartupUtils.ConfigureDapperContextBuilder(builder, identityConnectionString);
Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingBuilder(builder);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorBuilder(builder, identityConnectionString,
    Assembly.GetExecutingAssembly());
builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));

SqlMapper.AddTypeHandler(new MoneyValueHandler());
SqlMapper.AddTypeHandler(new DateOnlyHandler());

BindSystemServices(builder.Services);
BindRepositories(builder.Services);
BindAppServices(builder.Services);


builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5075, o => o.Protocols = HttpProtocols.Http2); });

var app = builder.Build();

Chassis.Grpc.StartupUtils.ConfigurePostgresLoggingApp(app);
Chassis.Grpc.StartupUtils.ConfigureFluentMigratorApp(app);

app.MapGrpcService<BankAccountGrpcService>();

app.Run();

void BindSystemServices(IServiceCollection services)
{
    services.AddCodeFirstGrpc();
    services.AddGrpc();
}

void BindRepositories(IServiceCollection services)
{
    services.AddScoped<IBankAccountRepository, PostgresBankAccountRepository>();
}

void BindAppServices(IServiceCollection services)
{
}
