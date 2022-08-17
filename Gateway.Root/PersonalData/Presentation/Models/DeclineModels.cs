using System.Runtime.Serialization;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class DeclinePersonalDataHttpRequest
{
	[DataMember] public Guid PersonalDataId { get; set; }
}

public class DeclinePersonalDataHttpResponse : GatewayHttpResponse
{
	public DeclinePersonalDataHttpResponse(string status, int statusCode) : base(status, statusCode)
	{
	}
}