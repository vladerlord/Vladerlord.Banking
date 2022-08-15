using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class SendPersonalDataConfirmationRequest
{
	[DataMember] public string FirstName { get; set; } = null!;
	[DataMember] public string LastName { get; set; } = null!;
	[DataMember] public string Country { get; set; } = null!;
	[DataMember] public string City { get; set; } = null!;
	[DataMember] public IFormFileCollection Files { get; set; } = null!;

	public PersonalDataConfirmationDto ToPersonalDataConfirmationDto(Guid userId)
	{
		return new PersonalDataConfirmationDto(userId, FirstName, LastName, Country, City, Files);
	}
}

[DataContract]
public class SendPersonalDataConfirmationResponse
{
	[DataMember] public string Status { get; }
	[DataMember] public int StatusCode { get; }
	[DataMember] public PersonalDataDto? PersonalData { get; }

	public SendPersonalDataConfirmationResponse(PersonalDataConfirmationResponseDto responseDto)
	{
		Status = responseDto.Status;
		StatusCode = responseDto.StatusCode;
		PersonalData = responseDto.PersonalData;
	}
}