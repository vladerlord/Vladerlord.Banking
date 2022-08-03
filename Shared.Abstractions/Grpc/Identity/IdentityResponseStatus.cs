namespace Shared.Abstractions.Grpc.Identity;

public enum IdentityResponseStatus
{
    Ok,
    Error,
    NotFound,
    UserAlreadyExist,
    Invalid
}
