using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.PersonalDataManagement.HttpModels;

[DataContract]
public class GetByIdRequest
{
    [DataMember] public Guid PersonalDataId { get; set; }
}

[DataContract]
public class GetByIdResponse : GatewayHttpResponse
{
    [DataMember] public PersonalDataDto? PersonalData { get; init; }
    [DataMember] public List<string> KycScansLinks { get; init; }

    public GetByIdResponse()
    {
        KycScansLinks = new List<string>();
    }
}
