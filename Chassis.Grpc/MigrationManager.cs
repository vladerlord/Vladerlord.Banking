using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chassis.Grpc;

public static class MigrationManager
{
    public static void MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        migrationService.ListMigrations();
        migrationService.MigrateUp();
    }
}