using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class DeclinePersonalDataGrpcRequest
{
	[DataMember(Order = 1)] public Guid PersonalDataId { get; set; }
}

[DataContract]
public class DeclinePersonalDataGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }

	public DeclinePersonalDataGrpcResponse()
	{
		GrpcResponse = new GrpcResponse();
	}
}