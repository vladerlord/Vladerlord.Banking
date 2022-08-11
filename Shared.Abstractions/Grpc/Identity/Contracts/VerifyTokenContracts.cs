using System.Runtime.Serialization;

namespace Shared.Abstractions.Grpc.Identity.Contracts;

[DataContract]
public class VerifyTokenGrpcRequest
{
	[DataMember(Order = 1)] public string JwtToken { get; set; } = null!;
}

[DataContract]
public class VerifyTokenGrpcResponse
{
	[DataMember(Order = 1)] public IdentityResponseStatus Status { get; set; }
}