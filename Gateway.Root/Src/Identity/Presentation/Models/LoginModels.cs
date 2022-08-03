using System.Net;
using Shared.Abstractions.Grpc.Identity;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Gateway.Root.Identity.Presentation.Models;

public class LoginUserHttpRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginUserHttpResponse
{
    public string Status { get; set; }
    public int StatusCode { get; }
    public string Jwt { get; set; }

    public LoginUserHttpResponse(IdentityResponseStatus status, string jwt)
    {
        Status = status.ToString();
        StatusCode = TransformStatus(status);
        Jwt = jwt;
    }

    private static int TransformStatus(IdentityResponseStatus status)
    {
        return status switch
        {
            IdentityResponseStatus.Ok => (int)HttpStatusCode.OK,
            IdentityResponseStatus.NotFound => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.BadRequest
        };
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