using Chassis;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Service.IdentityNotifier;
using Service.IdentityNotifier.Subscribers;

var environment = EnvironmentUtils.GetRequiredEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = environment.ToLower().Equals("development");

if (isDevelopment)
    SetEnvironmentVariablesFromUserSecrets();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, log) =>
    {
        log.Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", environment)
            .Filter.ByExcluding(c =>
                c.Properties.Any(p =>
                {
                    var (_, value) = p;
                    return value.ToString().Contains("swagger") || value.ToString().Contains("metrics");
                }))
            .WriteTo.Console();

        if (isDevelopment)
            log.MinimumLevel.Debug();
    })
    .ConfigureServices(services =>
    {
        ConfigureSmtp(services);
        ConfigureMasstransit(services);

        services.AddSingleton<RazorTemplateRenderer>();
        services.ConfigureOpenTelemetry();
    });

var app = builder.Build();

await app.RunAsync();

void ConfigureSmtp(IServiceCollection services)
{
    var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
    var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");
    var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
    var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
    var smtpFrom = Environment.GetEnvironmentVariable("SMTP_FROM");

    if (smtpHost == null || smtpPort == null || smtpUsername == null || smtpPassword == null || smtpFrom == null)
        throw new Exception("env SMTP_HOST/SMTP_PORT/SMTP_USERNAME/SMTP_PASSWORD/SMTP_FROM is not set");

    services.AddSingleton<IMailSender>(new MailSenderService(smtpHost, int.Parse(smtpPort), smtpUsername, smtpPassword,
        smtpFrom));
}

void ConfigureMasstransit(IServiceCollection services)
{
    services.AddMassTransit(bus =>
    {
        bus.AddConsumer<UserCreatedEventSubscriber>(typeof(UserCreatedEventSubscriberDefinition));

        bus.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));
        bus.UsingRabbitMq((context, config) =>
        {
            var hostUri = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
            var user = Environment.GetEnvironmentVariable("RABBITMQ_USER");
            var password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

            if (hostUri == null || user == null || password == null)
                throw new Exception($"env RABBITMQ_HOST/RABBITMQ_USER/RABBITMQ_PASSWORD is not set");

            config.Host(hostUri, "/", h =>
            {
                h.Username(user);
                h.Password(password);
            });

            config.ConfigureEndpoints(context);
        });
    });
}

void SetEnvironmentVariablesFromUserSecrets()
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

    foreach (var child in config.GetChildren())
        Environment.SetEnvironmentVariable(child.Key, child.Value);
}
