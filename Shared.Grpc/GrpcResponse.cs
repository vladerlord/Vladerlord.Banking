using System.Runtime.Serialization;

namespace Shared.Grpc;

[DataContract]
public class GrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponseStatus Status { get; init; }
}