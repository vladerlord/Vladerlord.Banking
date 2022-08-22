using System.Runtime.Serialization;

namespace Gateway.Root.Shared;

[DataContract]
public class GatewayHttpResponse
{
    [DataMember] public string? Status { get; set; }
}
