using Microsoft.AspNetCore.Http.Features;
using Shared.Abstractions.Grpc.Identity;
using Shared.Abstractions.Grpc.Identity.Contracts;

namespace Gateway.Root.Shared;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IIdentityGrpcService identityGrpcService)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<JwtAuthenticationAttribute>();

        if (attribute != null)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = (string)context.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");

                var response = await identityGrpcService.VerifyTokenAsync(new VerifyTokenGrpcRequest
                {
                    JwtToken = token
                });

                if (response.Status == IdentityResponseStatus.Invalid)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
        }

        await _next(context);
    }
}
