using System.Runtime.Serialization;
using Shared.Grpc;
using Shared.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Presentation.Models;

[DataContract]
public class LoginUserHttpRequest
{
	[DataMember] public string Email { get; }
	[DataMember] public string Password { get; }

	public LoginUserHttpRequest(string email, string password)
	{
		Email = email;
		Password = password;
	}
}

[DataContract]
public class LoginUserHttpResponse
{
	[DataMember] public string Status { get; }
	[DataMember] public int StatusCode { get; }
	[DataMember] public string? Jwt { get; }

	public LoginUserHttpResponse(GrpcResponseStatus status, string? jwt)
	{
		Status = status.ToString();
		StatusCode = status.ToHttpCode();
		Jwt = jwt;
	}
}

public static class LoginUserHttpExtensions
{
	public static LoginGrpcRequest ToGrpcRequest(this LoginUserHttpRequest request)
	{
		return new LoginGrpcRequest
		{
			Email = request.Email,
			Password = request.Password
		};
	}
}