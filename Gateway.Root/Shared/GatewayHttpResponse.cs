using System.Runtime.Serialization;

namespace Gateway.Root.Shared;

[DataContract]
public class GatewayHttpResponse
{
	[DataMember] public string Status { get; }
	[DataMember] public int StatusCode { get; }

	public GatewayHttpResponse(string status, int statusCode)
	{
		Status = status;
		StatusCode = statusCode;
	}
}