using System.Diagnostics.Metrics;
using System.Reflection;

namespace Chassis;

public static class Metrics
{
    public static readonly Meter Meter;

    static Metrics()
    {
        Meter = new Meter(GetServiceName());
    }

    public static string GetServiceName()
    {
        var assembly = Assembly.GetCallingAssembly();
        
        return assembly.GetName().Name ?? string.Empty;
    }
}
