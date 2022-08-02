using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class RegisterUserGrpcRequest
{
    [DataMember(Order = 1)] public string Email { get; set; }
    [DataMember(Order = 2)] public string Password { get; set; }
}

[DataContract]
public class RegisterUserGrpcResponse
{
    [DataMember(Order = 1)] public IdentityResponseStatus Status { get; set; }
    [DataMember(Order = 2)] public UserGrpcModel? UserModel { get; set; }
}
