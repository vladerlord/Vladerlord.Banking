using System.Net;
using System.Runtime.Serialization;
using Gateway.Root.PersonalData.Domain;
using Shared.Abstractions.Grpc;

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

	public SendPersonalDataConfirmationResponse(GrpcResponseStatus status)
	{
		Status = status.ToString();
		StatusCode = TransformStatus(status);
	}

	private static int TransformStatus(GrpcResponseStatus status)
	{
		return status switch
		{
			GrpcResponseStatus.Ok => (int)HttpStatusCode.OK,
			GrpcResponseStatus.Error => (int)HttpStatusCode.BadRequest,
			_ => (int)HttpStatusCode.BadRequest
		};
	}
}