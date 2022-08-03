using System.Net;
using Shared.Abstractions.Grpc.Identity;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Presentation.Models;

public class RegisterUserHttpRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterUserHttpResponse
{
    public string Status { get; set; }
    public int StatusCode { get; }
    
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

public static class RegisterUserHttpExtensions
{
    public static RegisterUserGrpcRequest ToGrpcRequest(this RegisterUserHttpRequest request)
    {
        return new RegisterUserGrpcRequest
        {
            Email = request.Email,
            Password = request.Password
        };
    }
}