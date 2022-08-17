using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using Shared.Grpc;
using Shared.Grpc.Identity;
using Shared.Grpc.Identity.Contracts;

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

		if (attribute == null)
		{
			await _next(context);
			return;
		}

		if (!context.Request.Headers.ContainsKey("Authorization"))
		{
			context.Response.StatusCode = 401;
			return;
		}

		var token = (string)context.Request.Headers["Authorization"];
		token = token.Replace("Bearer ", "");

		var response = await identityGrpcService.VerifyTokenAsync(new VerifyTokenGrpcRequest { JwtToken = token });

		if (response.Status == GrpcResponseStatus.Invalid || response.Claims == null)
		{
			context.Response.StatusCode = 401;
			return;
		}

		SetUser(context, response.Claims);

		await _next(context);
	}

	private static void SetUser(HttpContext context, IReadOnlyList<KeyValuePair<string, string>> responseClaims)
	{
		var claims = new Claim[responseClaims.Count];

		for (var i = 0; i < responseClaims.Count; i++)
			claims[i] = new Claim(responseClaims[i].Key, responseClaims[i].Value);

		context.User = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims) });
	}
}