using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Application;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class ListAllUnapprovedHttpResponse : GatewayHttpResponse
{
	[DataMember] public List<PersonalDataDto> PersonalDataList { get; }

	public ListAllUnapprovedHttpResponse(ListAllUnapprovedResponse responseDto)
		: base(responseDto.Status, responseDto.StatusCode)
	{
		PersonalDataList = responseDto.PersonalDataList;
	}
}