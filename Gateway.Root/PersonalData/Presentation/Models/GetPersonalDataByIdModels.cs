using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class GetPersonalDataByIdHttpRequest
{
	[DataMember] public Guid PersonalDataId { get; set; }
}

[DataContract]
public class GetPersonalDataByIdHttpResponse : GatewayHttpResponse
{
	[DataMember] public PersonalDataDto? PersonalData { get; }
	[DataMember] public List<string> KycScansLinks { get; }

	public GetPersonalDataByIdHttpResponse(GetPersonalDataByIdResponse response)
		: base(response.Status, response.StatusCode)
	{
		PersonalData = response.PersonalData;
		KycScansLinks = response.KycScansLinks;
	}
}