using System.Runtime.Serialization;

namespace Shared.Abstractions.Grpc.Identity.Contracts;

[DataContract]
public class RegisterConfirmationGrpcRequest
{
	[DataMember(Order = 1)] public string ConfirmationCode { get; set; } = null!;
}

[DataContract]
public class RegisterConfirmationGrpcResponse
{
	[DataMember(Order = 1)] public IdentityResponseStatus Status { get; set; }
}