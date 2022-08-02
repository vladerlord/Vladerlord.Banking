using BankingGateway.Shared;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Grpc.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidateModelStateAttribute));
}).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

BindGrpcServices(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtAuthenticationMiddleware>();

app.UseHttpsRedirection();

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
