using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Chassis.Gateway.ApiResponse;

public class ApiResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseWrapperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IOptions<HandleExceptionOptions> options)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<ApiResponseWrapperAttribute>();

        if (attribute == null)
        {
            await _next(context);
            return;
        }

        var currentBody = context.Response.Body;

        using var memoryStream = new MemoryStream();

        context.Response.Body = memoryStream;

        await _next(context);

        var handledByExceptionHandler = context.Response.StatusCode is 422 or 500;

        if (context.Response.HasStarted) return;

        context.Response.Body = currentBody;
        memoryStream.Seek(0, SeekOrigin.Begin);

        var contentAsString = await new StreamReader(memoryStream).ReadToEndAsync();

        if (handledByExceptionHandler)
        {
            await context.Response.WriteAsync(contentAsString);
            return;
        }

        var contentAsObject = JsonConvert.DeserializeObject(contentAsString);

        if (contentAsObject != null)
        {
            var response = new SuccessApiResponse<object>(contentAsObject, options.Value.ApiVersion,
                context.Response.StatusCode);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        else
        {
            await context.Response.WriteAsync(contentAsString);
        }
    }
}
