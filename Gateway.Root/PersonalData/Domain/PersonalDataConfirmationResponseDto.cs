using Shared.Grpc;

namespace Gateway.Root.PersonalData.Domain;

public class PersonalDataConfirmationResponseDto
{
	public string Status { get; }
	public int StatusCode { get; }
	public PersonalDataDto? PersonalData { get; }

	public PersonalDataConfirmationResponseDto(GrpcResponseStatus status, PersonalDataDto? personalData)
	{
		Status = status.ToString();
		StatusCode = status.ToHttpCode();
		PersonalData = personalData;
	}
}