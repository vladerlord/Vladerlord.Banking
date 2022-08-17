using System.Runtime.Serialization;
using Shared.Grpc.Identity.Models;

namespace Shared.Grpc.Identity.Contracts;

[DataContract]
public class RegisterUserGrpcRequest
{
	[DataMember(Order = 1)] public string Email { get; set; } = null!;
	[DataMember(Order = 2)] public string Password { get; set; } = null!;
	[DataMember(Order = 3)] public string ConfirmationCode { get; set; } = null!;
	[DataMember(Order = 4)] public string RegisterConfirmationUrl { get; set; } = null!;
}

[DataContract]
public class RegisterUserGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponseStatus Status { get; set; }
	[DataMember(Order = 2)] public UserGrpcModel? UserModel { get; set; }
}