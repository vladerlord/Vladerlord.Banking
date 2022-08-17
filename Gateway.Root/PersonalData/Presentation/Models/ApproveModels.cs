using System.Runtime.Serialization;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class ApprovePersonalDataHttpRequest
{
	[DataMember] public Guid PersonalDataId { get; set; }
}

public class ApprovePersonalDataHttpResponse : GatewayHttpResponse
{
	public ApprovePersonalDataHttpResponse(string status, int statusCode) : base(status, statusCode)
	{
	}
}