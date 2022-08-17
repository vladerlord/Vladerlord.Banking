using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ApprovePersonalDataGrpcRequest
{
	[DataMember(Order = 1)] public Guid PersonalDataId { get; set; }
}

[DataContract]
public class ApprovePersonalDataGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }

	public ApprovePersonalDataGrpcResponse()
	{
		GrpcResponse = new GrpcResponse();
	}
}