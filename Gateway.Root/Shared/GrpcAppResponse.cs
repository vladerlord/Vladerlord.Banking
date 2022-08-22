using System.Runtime.Serialization;
using Shared.Grpc;

namespace Gateway.Root.Shared;

[DataContract]
public class GrpcAppResponse<T>
{
    [DataMember] public GrpcResponse GrpcStatus { get; init; }
    [DataMember] public T Content { get; init; } = default!;

    public GrpcAppResponse()
    {
        GrpcStatus = new GrpcResponse();
    }
}

[DataContract]
public class GrpcAppResponse
{
    [DataMember] public GrpcResponse GrpcStatus { get; init; }

    public GrpcAppResponse()
    {
        GrpcStatus = new GrpcResponse();
    }
}
