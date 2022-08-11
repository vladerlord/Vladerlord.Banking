using System.Net;
using System.Runtime.Serialization;
using Shared.Abstractions.Grpc.Identity;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Presentation.Models;

[DataContract]
public class RegisterUserHttpRequest
{
	[DataMember] public string Email { get; }
	[DataMember] public string Password { get; }

	public RegisterUserHttpRequest(string email, string password)
	{
		Email = email;
		Password = password;
	}

	public RegisterUserGrpcRequest ToGrpcRequest(string confirmationCode, string confirmationUrl)
	{
		return new RegisterUserGrpcRequest
		{
			Email = Email,
			Password = Password,
			ConfirmationCode = confirmationCode,
			RegisterConfirmationUrl = confirmationUrl
		};
	}
}

[DataContract]
public class RegisterUserHttpResponse
{
	[DataMember] public string Status { get; }
	[DataMember] public int StatusCode { get; }

	public RegisterUserHttpResponse(IdentityResponseStatus status)
	{
		Status = status.ToString();
		StatusCode = TransformStatus(status);
	}

	private static int TransformStatus(IdentityResponseStatus status)
	{
		return status switch
		{
			IdentityResponseStatus.Ok => (int)HttpStatusCode.OK,
			IdentityResponseStatus.NotFound => (int)HttpStatusCode.NotFound,
			IdentityResponseStatus.UserAlreadyExist => (int)HttpStatusCode.Conflict,
			_ => (int)HttpStatusCode.BadRequest
		};
	}
}