using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;
using Shared.Grpc;

namespace Gateway.Root.PersonalData.Application;

[DataContract]
public class GetPersonalDataByIdResponse : ApplicationGrpcResponse
{
	[DataMember] public PersonalDataDto? PersonalData { get; }
	[DataMember] public List<string> KycScansLinks { get; }

	public GetPersonalDataByIdResponse(GrpcResponseStatus status, PersonalDataDto? personalData,
		List<string> kycScansLinks) : base(status)
	{
		PersonalData = personalData;
		KycScansLinks = kycScansLinks;
	}
}