using System.Runtime.Serialization;
using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Shared.Abstractions.Grpc.PersonalData.Contracts;

[DataContract]
public class ApplyPersonalDataGrpcRequest
{
	[DataMember(Order = 1)] public PersonalDataGrpcModel PersonalData { get; set; }
	[DataMember(Order = 2)] public List<KycScanGrpcModel> KycScans { get; set; }

	public ApplyPersonalDataGrpcRequest()
	{
		PersonalData = new PersonalDataGrpcModel();
		KycScans = new List<KycScanGrpcModel>();
	}

	public ApplyPersonalDataGrpcRequest(PersonalDataGrpcModel personalData, List<KycScanGrpcModel> kycScans)
	{
		PersonalData = personalData;
		KycScans = kycScans;
	}
}

[DataContract]
public class ApplyPersonalDataGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponseStatus Status { get; set; }

	public ApplyPersonalDataGrpcResponse()
	{
	}

	public ApplyPersonalDataGrpcResponse(GrpcResponseStatus status)
	{
		Status = status;
	}
}
