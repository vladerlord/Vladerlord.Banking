using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;

namespace Gateway.Root.PersonalData.Presentation.PersonalData.HttpModels;

[DataContract]
public class RequestApprovalRequest
{
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Country { get; set; }
    [DataMember] public string City { get; set; }
    [DataMember] public IFormFileCollection Files { get; set; }

    public RequestApprovalRequest()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Country = string.Empty;
        City = string.Empty;
        Files = new FormFileCollection();
    }

    public PersonalDataConfirmationDto ToPersonalDataConfirmationDto(Guid userId)
    {
        return new PersonalDataConfirmationDto(userId, FirstName, LastName, Country, City, Files);
    }
}

[DataContract]
public class RequestApprovalResponse : GatewayHttpResponse
{
    [DataMember] public PersonalDataDto? PersonalData { get; init; }
}
