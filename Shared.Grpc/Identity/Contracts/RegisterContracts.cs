using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class RegisterUserGrpcRequest
{
    [DataMember(Order = 1)] public string Email { get; init; }
    [DataMember(Order = 2)] public string Password { get; init; }
    [DataMember(Order = 3)] public string ConfirmationCode { get; init; }
    [DataMember(Order = 4)] public string RegisterConfirmationUrl { get; init; }

    public RegisterUserGrpcRequest()
    {
        Email = string.Empty;
        Password = string.Empty;
        ConfirmationCode = string.Empty;
        RegisterConfirmationUrl = string.Empty;
    }
}

[DataContract]
public class RegisterUserGrpcResponse
{
    [DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; init; }
    [DataMember(Order = 2)] public UserGrpcModel? UserModel { get; init; }

    public RegisterUserGrpcResponse()
    {
        GrpcResponse = new GrpcResponse();
    }
}
