using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Shared.Abstractions;

namespace Chassis.Gateway;

public class AdminPermissionRequiredMiddleware
{
	private readonly RequestDelegate _next;

	public AdminPermissionRequiredMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
		var attribute = endpoint?.Metadata.GetMetadata<AdminPermissionRequiredAttribute>();

		if (attribute == null)
		{
			await _next(context);
			return;
		}

		if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserStatus.Admin.AsString()))
		{
			context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			return;
		}

		await _next(context);
	}
}