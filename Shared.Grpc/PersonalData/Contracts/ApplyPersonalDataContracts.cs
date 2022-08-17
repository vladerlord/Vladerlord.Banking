using System.Runtime.Serialization;
using Shared.Grpc.PersonalData.Models;

namespace Shared.Grpc.PersonalData.Contracts;

[DataContract]
public class ApplyPersonalDataGrpcRequest
{
	public ApplyPersonalDataGrpcRequest()
	{
		PersonalData = new PersonalDataCreateGrpcModel();
		KycScans = new List<KycScanCreateGrpcModel>();
	}

	public ApplyPersonalDataGrpcRequest(PersonalDataCreateGrpcModel personalData, List<KycScanCreateGrpcModel> kycScans)
	{
		PersonalData = personalData;
		KycScans = kycScans;
	}

	[DataMember(Order = 1)] public PersonalDataCreateGrpcModel PersonalData { get; set; }
	[DataMember(Order = 2)] public List<KycScanCreateGrpcModel> KycScans { get; set; }
}

[DataContract]
public class ApplyPersonalDataGrpcResponse
{
	[DataMember(Order = 1)] public GrpcResponseStatus Status { get; set; }
	[DataMember(Order = 2)] public PersonalDataGrpcModel? PersonalData { get; set; }
}