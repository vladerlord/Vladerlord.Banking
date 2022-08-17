using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ListAllUnapprovedPersonalDataGrpcRequest
{
}

[DataContract]
public class ListAllUnapprovedPersonalDataGrpcResponse
{
	public ListAllUnapprovedPersonalDataGrpcResponse()
	{
		PersonalDataList = new List<PersonalDataGrpcModel>();
	}

	public ListAllUnapprovedPersonalDataGrpcResponse(GrpcResponseStatus status,
		List<PersonalDataGrpcModel> personalDataList)
	{
		Status = status;
		PersonalDataList = personalDataList;
	}

	[DataMember(Order = 1)] public GrpcResponseStatus Status { get; set; }
	[DataMember(Order = 2)] public List<PersonalDataGrpcModel> PersonalDataList { get; set; }
}