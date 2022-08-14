namespace Shared.Abstractions.Grpc;

public enum GrpcResponseStatus
{
	Ok,
	Error,
	NotFound,
	UserAlreadyExist,
	Invalid
}