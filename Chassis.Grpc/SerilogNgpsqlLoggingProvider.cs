using Microsoft.Extensions.DependencyInjection;
using Npgsql.Logging;

namespace Chassis.Grpc;

public class SerilogNgpsqlLoggingProvider : INpgsqlLoggingProvider
{
    private readonly IServiceProvider _serviceProvider;

    public SerilogNgpsqlLoggingProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public NpgsqlLogger CreateLogger(string name)
    {
        var logger = _serviceProvider.GetService<SerilogNpgsqlLogger>();

        if (logger == null)
            throw new Exception($"{nameof(SerilogNpgsqlLogger)} is not defined for DI");

        return logger;
    }
}
