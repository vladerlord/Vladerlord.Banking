using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;

namespace Chassis.Gateway;

public static class GrpcUtils
{
    public static void AddGrpcService<T>(this IServiceCollection services, string connection,
        Action<GrpcClientFactoryOptions>? callback = null) where T : class
    {
        services.AddCodeFirstGrpcClient<T>(o =>
        {
            o.Address = new Uri(connection);

            callback?.Invoke(o);
        });
    }
}
