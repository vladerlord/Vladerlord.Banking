using Chassis;
using Chassis.Gateway;
using Chassis.Gateway.ApiResponse;
using FluentValidation.AspNetCore;
using Gateway.Root.Identity.Application;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.Grpc.BankAccount;
using Shared.Grpc.Identity;
using Shared.Grpc.PersonalData;

var builder = WebApplication.CreateBuilder(args);

StartupUtils.ConfigureLogging(builder);

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(typeof(ValidateModelStateAttribute));
        options.Filters.Add(typeof(HandleExceptionAttribute));
    })
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddMvcCore()
    .AddRazorViewEngine();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.Configure<HandleExceptionOptions>(options => options.ApiVersion = "1.0.0");
builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));

BindSystemServices(builder.Services);
BindAppServices(builder.Services);
BindGrpcServices(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<JwtAuthenticationMiddleware>();
app.UseMiddleware<AdminPermissionRequiredMiddleware>();
app.UseMiddleware<ApiResponseWrapperMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.Run();

void BindSystemServices(IServiceCollection services)
{
    services.AddSingleton(Log.Logger);
}

void BindGrpcServices(WebApplicationBuilder weBuilder)
{
    var identityGrpc = EnvironmentUtils.GetRequiredEnvironmentVariable("IDENTITY_GRPC");
    var personalDataGrpc = EnvironmentUtils.GetRequiredEnvironmentVariable("PERSONAL_DATA_GRPC");
    var bankAccountGrpc = EnvironmentUtils.GetRequiredEnvironmentVariable("BANK_ACCOUNT_GRPC");

    weBuilder.Services.AddGrpcService<IIdentityGrpcService>(identityGrpc);
    weBuilder.Services.AddGrpcService<IPersonalDataGrpcService>(personalDataGrpc);
    weBuilder.Services.AddGrpcService<IKycScanGrpcService>(personalDataGrpc);
    weBuilder.Services.AddGrpcService<IBankAccountGrpcService>(bankAccountGrpc);
}

void BindAppServices(IServiceCollection services)
{
    var domain = EnvironmentUtils.GetRequiredEnvironmentVariable("DOMAIN");

    services.AddScoped(provider => new KycScanLinkBuilder(domain, provider.GetRequiredService<LinkGenerator>()));
    services.AddScoped(provider => new UserResetService(domain, provider.GetRequiredService<LinkGenerator>()));
    services.AddScoped<PersonalDataService>();
    services.AddScoped<PersonalDataManagementService>();
    services.AddScoped<UserService>();
}
