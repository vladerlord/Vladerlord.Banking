using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class LoginGrpcRequest
{
    [DataMember(Order = 1)] public string Email { get; set; }
    [DataMember(Order = 2)] public string Password { get; set; }
}

[DataContract]
public class LoginGrpcResponse
{
    [DataMember(Order = 1)] public IdentityResponseStatus Status { get; set; }
    [DataMember(Order = 2)] public UserGrpcModel UserModel { get; set; }
    [DataMember(Order = 3)] public string Jwt { get; set; }
}