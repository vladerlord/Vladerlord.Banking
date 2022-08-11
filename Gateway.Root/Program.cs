using Chassis.Gateway;
using FluentValidation.AspNetCore;
using Gateway.Root.Identity.Application;
using Gateway.Root.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Grpc.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddControllers(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); })
	.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddMvcCore()
	.AddRazorViewEngine();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddScoped(provider =>
{
	var domain = Environment.GetEnvironmentVariable("DOMAIN");

	if (domain == null)
		throw new Exception("env DOMAIN is not set");

	return new UserResetService(domain, provider.GetRequiredService<LinkGenerator>());
});

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

app.UseAuthorization();

app.MapControllers();
app.Run();

void BindGrpcServices(WebApplicationBuilder weBuilder)
{
	var identityGrpc = Environment.GetEnvironmentVariable("IDENTITY_GRPC");

	if (identityGrpc == null)
		throw new Exception("IDENTITY_GRPC is not set");

	weBuilder.Services.AddGrpcService<IIdentityGrpcService>(identityGrpc);
}