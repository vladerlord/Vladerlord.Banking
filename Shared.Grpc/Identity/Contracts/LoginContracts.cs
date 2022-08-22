using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class LoginGrpcRequest
{
    [DataMember(Order = 1)] public string Email { get; init; }
    [DataMember(Order = 2)] public string Password { get; init; }

    public LoginGrpcRequest()
    {
        Email = string.Empty;
        Password = string.Empty;
    }
}

[DataContract]
public class LoginGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public UserGrpcModel? UserModel { get; init; }
    [DataMember(Order = 3)] public string? Jwt { get; init; }

    public LoginGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
