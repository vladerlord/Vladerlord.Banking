namespace Shared.Grpc.Identity;

public enum IdentityResponseStatus
{
    Ok,
    Error,
    NotFound,
    UserAlreadyExist,
    Invalid
}
