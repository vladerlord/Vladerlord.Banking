using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.PersonalDataManagement.HttpModels;

[DataContract]
public class GetAllUnapprovedResponse : GatewayHttpResponse
{
    [DataMember] public IEnumerable<PersonalDataDto> PersonalDataList { get; init; }

    public GetAllUnapprovedResponse()
    {
        PersonalDataList = new List<PersonalDataDto>();
    }
}
