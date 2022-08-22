using System.Runtime.Serialization;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class VerifyTokenGrpcRequest
{
    [DataMember(Order = 1)] public string JwtToken { get; init; }

    public VerifyTokenGrpcRequest()
    {
        JwtToken = string.Empty;
    }
}

[DataContract]
public class VerifyTokenGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponseStatus Status { get; init; }
    [DataMember(Order = 2)] public List<KeyValuePair<string, string>>? Claims { get; init; }
}
