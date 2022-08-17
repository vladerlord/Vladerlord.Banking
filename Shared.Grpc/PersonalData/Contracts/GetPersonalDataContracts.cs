using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class GetPersonalDataByIdGrpcRequest
{
	public GetPersonalDataByIdGrpcRequest()
	{
	}

	public GetPersonalDataByIdGrpcRequest(Guid personalDataId)
	{
		PersonalDataId = personalDataId;
	}

	[DataMember(Order = 1)] public Guid PersonalDataId { get; }
}

[DataContract]
public class GetPersonalDataByIdGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponse GrpcResponse { get; set; }
	[DataMember(Order = 2)] public PersonalDataGrpcModel? PersonalData { get; set; }
	[DataMember(Order = 3)] public List<Guid> KycScansIds { get; set; }

	public GetPersonalDataByIdGrpcResponse()
	{
		KycScansIds = new List<Guid>();
		GrpcResponse = new GrpcResponse();
	}
}