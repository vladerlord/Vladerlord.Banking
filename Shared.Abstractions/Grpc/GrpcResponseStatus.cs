using System.Net;

namespace Shared.Abstractions.Grpc;

public enum GrpcResponseStatus
{
	Ok,
	Error,
	NotFound,
	UserAlreadyExist,
	Invalid
}

public static class GrpcResponseStatusExtenstion
{
	public static int ToHttpCode(this GrpcResponseStatus status)
	{
		return status switch
		{
			GrpcResponseStatus.Ok => (int)HttpStatusCode.OK,
			GrpcResponseStatus.NotFound => (int)HttpStatusCode.NotFound,
			GrpcResponseStatus.UserAlreadyExist => (int)HttpStatusCode.Conflict,
			_ => (int)HttpStatusCode.BadRequest
		};
	}
}