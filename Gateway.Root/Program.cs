using Chassis;
using Chassis.Gateway;
using FluentValidation.AspNetCore;
using Gateway.Root.Identity.Application;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Grpc.Identity;
using Shared.Grpc.PersonalData;

var builder = WebApplication.CreateBuilder(args);

StartupUtils.ConfigureLogging(builder);

builder.Services
	.AddControllers(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); })
	.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddMvcCore()
	.AddRazorViewEngine();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

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

app.UseAuthorization();

app.MapControllers();
app.Run();

void BindGrpcServices(WebApplicationBuilder weBuilder)
{
	var identityGrpc = EnvironmentUtils.GetRequiredEnvironmentVariable("IDENTITY_GRPC");
	var personalDataGrpc = EnvironmentUtils.GetRequiredEnvironmentVariable("PERSONAL_DATA_GRPC");

	weBuilder.Services.AddGrpcService<IIdentityGrpcService>(identityGrpc);
	weBuilder.Services.AddGrpcService<IPersonalDataGrpcService>(personalDataGrpc);
	weBuilder.Services.AddGrpcService<IKycScanGrpcService>(personalDataGrpc);
}

void BindAppServices(IServiceCollection services)
{
	var domain = EnvironmentUtils.GetRequiredEnvironmentVariable("DOMAIN");

	services.AddScoped(provider => new KycScanLinkBuilder(domain, provider.GetRequiredService<LinkGenerator>()));
	services.AddScoped(provider => new UserResetService(domain, provider.GetRequiredService<LinkGenerator>()));
	services.AddScoped<PersonalDataService>();
	services.AddScoped<PersonalDataManagementService>();
}