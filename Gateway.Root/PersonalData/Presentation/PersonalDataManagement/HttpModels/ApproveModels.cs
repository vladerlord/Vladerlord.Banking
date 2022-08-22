using System.Runtime.Serialization;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.PersonalDataManagement.HttpModels;

[DataContract]
public class ApproveRequest
{
    [DataMember] public Guid PersonalDataId { get; set; }
}

public class ApproveResponse : GatewayHttpResponse
{
}
